using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace XamarinAndroid.SmsForward
{
    [Service]
    internal class SMSForwardService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private SMSBroadcastReceiver mReceiver;

        public override void OnCreate()
        {
            base.OnCreate();
            mReceiver = new SMSBroadcastReceiver();
        }

        private const int ServiceNotificationId = 20190609;
        private const string SmsIntentFilter = "android.provider.Telephony.SMS_RECEIVED";
        private const string NotificationChannelId = "198964";

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (intent == null)
            {
                StopAll();
                return StartCommandResult.NotSticky;
            }

            // if user starts the service
            switch (intent.Action)
            {
                case ServiceAction.Start:
                    CreateNotificationChannel();
                    StartForeground(ServiceNotificationId, CreateNotification());

                    RegisterReceiver(mReceiver, new IntentFilter(SmsIntentFilter));
                    Toast.MakeText(this, "SMS Broadcast receiver registered.", ToastLength.Long).Show();

                    //=======you can do your always running work here.=====
                    //var startTimeSpan = TimeSpan.Zero;
                    //var periodTimeSpan = TimeSpan.FromHours(1);

                    //var timer = new System.Threading.Timer((e) =>
                    //{
                    //    //do your work
                    //}, null, startTimeSpan, periodTimeSpan);

                    Toast.MakeText(this, "Service started.", ToastLength.Long).Show();

                    break;
                case ServiceAction.Stop:
                    StopAll();
                    Toast.MakeText(this, "Service stopped.", ToastLength.Long).Show();
                    break;
                default:
                    StopAll();
                    break;
            }

            return StartCommandResult.NotSticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAll();
        }

        private void StopAll()
        {
            try
            {
                UnregisterReceiver(mReceiver);
                Toast.MakeText(this, "SMS Broadcast receiver unregistered.", ToastLength.Long).Show();
            }
            catch { }
            StopForeground(true);
            StopSelf();
        }

        private void CreateNotificationChannel()
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (notificationManager.GetNotificationChannel(NotificationChannelId) == null)
            {
                var channelName = Resources.GetString(Resource.String.app_name);
                var channelDescription = "SMS Forwarding Service";
                var channel = new NotificationChannel(NotificationChannelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                channel.EnableVibration(false);

                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private Notification CreateNotification()
        {
            string messageBody = "SMS Forwarding Service Running";
            string messageTitle = "Foreground";

            // / Create an Intent for the activity you want to start
            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ServiceAction.Stop);
            notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.Immutable);

            // notification builder
            var notificationBuilder = new Notification.Builder(this, NotificationChannelId);

            notificationBuilder
                 .SetContentIntent(pendingIntent)
                 .SetContentTitle(messageTitle)
                 .SetContentText(messageBody)
                 .SetSmallIcon(Resource.Drawable.ic_fgs)
                 .SetOngoing(true);

            return notificationBuilder.Build();
        }
    }
}