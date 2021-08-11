using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using DangerZone.Droid.Models;
using Microcharts;
using Microcharts.Droid;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Orientation = Microcharts.Orientation;

namespace DangerZone.Droid.Fragments
{
    public class Dashboard : Fragment
    {
        Spinner spinner;
        HttpClient client;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            client = new HttpClient();

        }

        public static Dashboard NewInstance()
        {
            var frag = new Dashboard { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.dashboard, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            spinner = view.FindViewById<Spinner>(Resource.Id.spinner);

            spinner.ItemSelected += Spinner_ItemSelected;

            GetStates();
        }

        private async void GetStates()
        {
            var response = await client.GetStringAsync(@"https://covidtracking.com/api/states/info");
            var result = JsonConvert.DeserializeObject<List<StateInfo>>(response);
            if (Context != null)
            {
                var adapter = new ArrayAdapter<string>(Context, Android.Resource.Layout.SimpleSpinnerItem);
                adapter.AddAll(result.Select(x => x.Name).ToList());
                spinner.Adapter = adapter;
            }
        }

        private async void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            var selected = spinner.GetItemAtPosition(e.Position);
            var response = await client.GetStringAsync(@$"https://corona.lmao.ninja/v2/states/{selected}");
            var result = JsonConvert.DeserializeObject<StateDetail>(response);
            if (Context != null)
            {
                var charts = CreateQuickstart(result);

                View.FindViewById<ChartView>(Resource.Id.chartView1).Chart = charts[0];
                View.FindViewById<ChartView>(Resource.Id.chartView2).Chart = charts[1];
                View.FindViewById<ChartView>(Resource.Id.chartView3).Chart = charts[2];
                View.FindViewById<ChartView>(Resource.Id.chartView4).Chart = charts[3];
            }
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
    }
}