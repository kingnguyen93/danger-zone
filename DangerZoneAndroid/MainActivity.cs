using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using DangerZoneAndroid.Fragments;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace DangerZoneAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoTitleBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationBarView.IOnItemSelectedListener
    {
        private BottomNavigationView navigation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SelectedItemId = Resource.Id.navigation_dashboard;

            navigation.SetOnItemSelectedListener(this);

            LoadFragment(Resource.Id.navigation_dashboard);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            return LoadFragment(item.ItemId);
        }

        bool LoadFragment(int id)
        {
            Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.navigation_home:
                    fragment = Home.NewInstance();
                    break;

                case Resource.Id.navigation_dashboard:
                    fragment = Dashboard.NewInstance();
                    break;

                case Resource.Id.navigation_notifications:
                    fragment = Notifications.NewInstance();
                    break;
            }

            if (fragment == null)
                return false;

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            return true;
        }
    }
}