using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Android.Widget;
using Android.Content;

namespace XamarinAndroid.SmsForward
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            TextView txt = FindViewById<TextView>(Resource.Id.textView1);
            txt.Text = "SMS Forwarding Service";

            Button btn1 = FindViewById<Button>(Resource.Id.button1);
            Button btn2 = FindViewById<Button>(Resource.Id.button2);
            btn1.Click += Btn1_Click;
            btn2.Click += Btn2_Click;
        }

        private Intent startIntent;

        private void Btn2_Click(object sender, EventArgs e)
        {
            // stop foreground service.
            Toast.MakeText(this, "Stopping service, please wait...", ToastLength.Long).Show();
            Intent stopIntent = new Intent(this, typeof(SMSForwardService));
            stopIntent.SetAction(ServiceAction.Stop);
            StartService(stopIntent);
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "Starting service, please wait...", ToastLength.Long).Show();
            startIntent = new Intent(this, typeof(SMSForwardService));
            startIntent.SetAction(ServiceAction.Start);

            // start foreground service.
            StartForegroundService(startIntent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
