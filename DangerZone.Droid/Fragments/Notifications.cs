using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DangerZone.Droid.Fragments
{
    public class Notifications : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static Notifications NewInstance()
        {
            var frag = new Notifications { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);

            // Use this to return your custom view for this Fragment
            return inflater.Inflate(Resource.Layout.notifications, container, false);
        }
    }
}