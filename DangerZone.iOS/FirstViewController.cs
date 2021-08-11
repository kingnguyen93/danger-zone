using CoreGraphics;
using DangerZone.iOS.Models;
using Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UIKit;

namespace DangerZone.iOS
{
    public partial class FirstViewController : UIViewController
    {
        private readonly HttpClient client;
        private List<StateInfo> states;

        private UITableView tableView;

        public FirstViewController(IntPtr handle) : base(handle)
        {
            client = new HttpClient();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            tableView = new UITableView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height));

            View.AddSubview(tableView);

            GetData();
        }

        private async void GetData()
        {
            var response = await client.GetStringAsync(@"https://corona.lmao.ninja/v2/states");
            var result = JsonConvert.DeserializeObject<List<StateDetail>>(response);

            var source = new TableViewSource(result);
            tableView.Source = source;

            source.ValueChanged += (s, e) =>
            {
                var selected = states[source.SelectedIndex];
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }

    public class TableViewSource : UITableViewSource
    {
        public event EventHandler<EventArgs> ValueChanged;

        public int SelectedIndex { get; set; }

        private readonly List<StateDetail> data;

        public TableViewSource(List<StateDetail> data)
        {
            this.data = data;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cell") as CustomCell;
            if (cell == null)
                cell = new CustomCell(new NSString("cell"));
            cell.UpdateCell(
                data[indexPath.Row].State,
                $"New: {data[indexPath.Row].TodayCases}. Active: {data[indexPath.Row].Active}",
                UIImage.FromFile("ic_arrow_forward.png")
                );
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return data.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            SelectedIndex = indexPath.Row;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == data.Count - 1)
            {
                //Reload your data here
            }
        }
    }

    public class CustomCell : UITableViewCell
    {
        private readonly UILabel lblTitle;
        private readonly UILabel lblDescription;
        private readonly UIImageView imageView;

        public CustomCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            lblTitle = new UILabel()
            {
                Font = UIFont.BoldSystemFontOfSize(16),
                BackgroundColor = UIColor.Clear
            };
            lblDescription = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14),
                BackgroundColor = UIColor.Clear
            };
            imageView = new UIImageView();
            ContentView.AddSubviews(new UIView[] { lblTitle, lblDescription, imageView });
        }

        public void UpdateCell(string title, string description, UIImage image)
        {
            lblTitle.Text = title;
            lblDescription.Text = description;
            imageView.Image = image;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            lblTitle.Frame = new CGRect(5, 2, ContentView.Bounds.Width - 40, 20);
            lblDescription.Frame = new CGRect(5, 22, ContentView.Bounds.Width - 40, 20);
            imageView.Frame = new CGRect(ContentView.Bounds.Width - 40, 10, 25, 25);
        }
    }
}