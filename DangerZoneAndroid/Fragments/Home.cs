using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using DangerZoneAndroid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DangerZoneAndroid.Fragments
{
    public class Home : Fragment
    {
        ListView listView;
        HttpClient client;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            client = new HttpClient();
        }

        public static Home NewInstance()
        {
            var frag = new Home { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.home, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            listView = view.FindViewById<ListView>(Resource.Id.listview);

            listView.ItemSelected += ListView_ItemSelected;

            GetData();
        }

        private async void GetData()
        {
            var response = await client.GetStringAsync(@"https://corona.lmao.ninja/v2/states");
            var result = JsonConvert.DeserializeObject<List<StateDetail>>(response);
            if (Context != null)
            {
                var adapter = new ListViewAdapter(Activity, result);
                listView.Adapter = adapter;
            }
        }

        private void ListView_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
        }
    }

    public class ListViewAdapter : BaseAdapter<StateDetail>
    {
        Android.App.Activity context;
        List<StateDetail> items;

        public ListViewAdapter(Android.App.Activity context, List<StateDetail> items) : base()
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public override StateDetail this[int position] => items[position];

        public override int Count => items.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.State;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = $"New: {item.TodayCases}. Active: {item.Active}";
            return view;
        }
    }
}