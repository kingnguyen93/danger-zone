using CoreGraphics;
using DangerZone.iOS.Models;
using Microcharts;
using Microcharts.iOS;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UIKit;

namespace DangerZone.iOS
{
    public partial class SecondViewController : UIViewController
    {
        private readonly HttpClient client;

        private UIScrollView scrollView;
        private UIPickerView pickerView;
        private ChartView chartView1;
        private ChartView chartView2;
        private ChartView chartView3;
        private ChartView chartView4;

        private List<StateInfo> states;

        public SecondViewController(IntPtr handle) : base(handle)
        {
            client = new HttpClient();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            View.BackgroundColor = UIColor.White;

            scrollView = new UIScrollView()
            {
                BackgroundColor = UIColor.White,
                Frame = new CGRect(0, 0, View.Frame.Width, 1080),
                ContentSize = new CGSize(View.Frame.Width, 1080),
                AutoresizingMask = UIViewAutoresizing.FlexibleHeight
            };
            // View.AddSubview(scrollView);
            View = scrollView;

            pickerView = new UIPickerView(new CGRect(0, 0, View.Bounds.Width, 180));
            pickerView.ShowSelectionIndicator = true;

            scrollView.AddSubview(pickerView);

            chartView1 = new ChartView
            {
                Frame = new CGRect(0, 200, View.Bounds.Width, 200),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            chartView2 = new ChartView
            {
                Frame = new CGRect(0, 420, View.Bounds.Width, 200),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            chartView3 = new ChartView
            {
                Frame = new CGRect(0, 640, View.Bounds.Width, 200),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            chartView4 = new ChartView
            {
                Frame = new CGRect(0, 860, View.Bounds.Width, 200),
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth
            };

            scrollView.AddSubviews(chartView1, chartView2, chartView3, chartView4);

            GetStates();
        }

        private async void GetStates()
        {
            var response = await client.GetStringAsync(@"https://covidtracking.com/api/states/info");
            states = JsonConvert.DeserializeObject<List<StateInfo>>(response);

            var picker = new StateInfoModel(states);
            pickerView.Model = picker;

            LoadDetail(states[0].Name);

            picker.ValueChanged += (s, e) =>
            {
                var selected = states[picker.SelectedIndex];
                LoadDetail(selected.Name);
            };
        }

        private async void LoadDetail(string state)
        {
            var response2 = await client.GetStringAsync($@"https://corona.lmao.ninja/v2/states/{state}");
            var result = JsonConvert.DeserializeObject<StateDetail>(response2);

            var charts = CreateQuickstart(result);

            chartView1.Chart = charts[0];
            chartView2.Chart = charts[1];
            chartView3.Chart = charts[2];
            chartView4.Chart = charts[3];
        }

        private Chart[] CreateQuickstart(StateDetail result)
        {
            var entries = new[]
            {
                new ChartEntry(200)
                {
                        Label = "New",
                        ValueLabel = result.TodayCases.ToString(),
                        Color = SKColor.Parse("#266489")
                },
                new ChartEntry(400)
                {
                        Label = "Active",
                        ValueLabel = result.Active.ToString(),
                        Color = SKColor.Parse("#68B9C0")
                }
            };

            return new Chart[]
            {
                new BarChart
                {
                    Entries = entries,
                    LabelTextSize = 32,
                    LabelOrientation = Orientation.Horizontal,
                    Margin = 10
                },
                new PointChart
                {
                    Entries = entries,
                    LabelTextSize = 32,
                    LabelOrientation = Orientation.Horizontal,
                    Margin = 10
                },
                new DonutChart
                {
                    Entries = entries,
                    LabelTextSize = 32,
                },
                new RadialGaugeChart
                {
                    Entries = entries,
                    LabelTextSize = 32
                }
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }

    public class StateInfoModel : UIPickerViewModel
    {
        public event EventHandler<EventArgs> ValueChanged;

        public int SelectedIndex { get; set; }

        public List<StateInfo> data;

        public StateInfoModel(List<StateInfo> data)
        {
            this.data = data;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return data.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (component == 0)
                return data[(int)row].Name;
            else
                return row.ToString();
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            SelectedIndex = (int)row;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public override nfloat GetComponentWidth(UIPickerView picker, nint component)
        {
            if (component == 0)
                return 240f;
            else
                return 40f;
        }

        public override nfloat GetRowHeight(UIPickerView picker, nint component)
        {
            return 40f;
        }
    }
}