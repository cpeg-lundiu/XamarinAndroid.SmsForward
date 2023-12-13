using Android.Content;
using Android.Telephony;
using Android.Widget;
using System;
using System.Text;

namespace XamarinAndroid.SmsForward
{
    [BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
    internal class SMSBroadcastReceiver : BroadcastReceiver
    {
        public static readonly string IntentAction = "android.provider.Telephony.SMS_RECEIVED";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != IntentAction) return;

            var bundle = intent.Extras;

            if (bundle != null)
            {
                var stringBuilder = new StringBuilder();

                try
                {
                    var pdus = (Java.Lang.Object[])bundle.Get("pdus");
                    var messages = new SmsMessage[pdus.Length];

                    var sender = string.Empty;

                    for (int i = 0; i < messages.Length; i++)
                    {
                        messages[i] = SmsMessage.CreateFromPdu((byte[])pdus[i]);

                        if (messages[i].OriginatingAddress != sender)
                        {
                            if (!string.IsNullOrEmpty(sender))
                                stringBuilder.Append(Environment.NewLine);

                            sender = messages[i].OriginatingAddress;
                            stringBuilder.AppendFormat("From : {0}" + Environment.NewLine + "{1}",
                                sender, messages[i].MessageBody.ToString());
                        }
                        else
                        {
                            stringBuilder.Append(messages[i].MessageBody.ToString());
                        }
                    }

                    Toast.MakeText(context, stringBuilder.ToString(), ToastLength.Long).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                }

                if (stringBuilder.Length >= 0)
                {
                    var mailsender = new EmailSender();

                    try
                    {
                        mailsender.SendEmail(stringBuilder.ToString());
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                    }
                    finally
                    {
                        mailsender.Dispose();
                    }
                }
            }
        }
    }
}