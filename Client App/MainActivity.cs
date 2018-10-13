using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Locations;
using Android.Util;
using Android.Content;
using Android.Views;
using System.Collections.Generic;
using System.Linq;
using Plugin.Geolocator;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;

namespace Client_App
{
    [Activity(Label = "Golf Tracer", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView GPSLocationLongitude;
        TextView GPSLocationLatitude;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            TextView label = (TextView)FindViewById(Resource.Id.textView1);
            Random rand = new Random();
            label.Text = rand.Next(111111, 999999).ToString();

            GPSLocationLongitude = (TextView)FindViewById(Resource.Id.textView2);
            GPSLocationLatitude = (TextView)FindViewById(Resource.Id.textView3);

            UpdateGPS();
            
        }
        private async void UpdateGPS()
        {
            await TryToGetPermissions();
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync(timeout: TimeSpan.FromMilliseconds(100000));

                GPSLocationLongitude.Text = position.Longitude.ToString();
                GPSLocationLatitude.Text = position.Latitude.ToString();
            }
            catch (Exception e)
            {
                GPSLocationLongitude.Text = "Error:";
                GPSLocationLatitude.Text = e.ToString();
            }
        }

        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }


        }
        const int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
            {
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessFineLocation,
             };
        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                Toast.MakeText(this, "GPS permissions granted", ToastLength.Short).Show();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Permissions Needed");
                alert.SetMessage("The application needs GPS to continue");
                alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                {
                    RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled", ToastLength.Short).Show();
                });
                Dialog dialog = alert.Create();
                dialog.Show();

                return;
            }

            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "GPS permissions granted", ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, "GPS permissions denied", ToastLength.Short).Show();
                        }
                    }
                    break;
            }
            
        }
    }
}