namespace Zebble.Device
{
    using Android.Content;
    using Context = Android.Content.Context;
    using Newtonsoft.Json;
    using System;
    using Olive;
    using System.IO;

    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Plugin Broadcast Receiver")]
    public class ScheduledNotificationsBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Android.Widget.Toast.MakeText(UIRuntime.CurrentActivity, $"OnRecieved {DateTime.Now.ToString()}", Android.Widget.ToastLength.Short).Show();
                var extra = intent.GetStringExtra(LocalNotification.LocalNotificationKey);
                if (extra.IsEmpty()) return;

                var notification = JsonConvert.DeserializeObject<AndroidLocalNotification>(extra);
                if (notification is null) return;


                //LocalNotification.Show(notification);
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex);
            }
        }
    }
}