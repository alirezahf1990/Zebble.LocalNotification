﻿namespace Zebble.Device
{
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Newtonsoft.Json;
    using Olive;
    using System;

    public static partial class LocalNotification
    {
        internal const string LocalNotificationKey = "LocalNotification";

        static AndroidNotificationIcon Icon;
        static AndroidNotificationIcon TransparentIcon;
        static Color TransparentIconColor = Colors.White;
        static Action<Notification> OnTapped;

        static NotificationManager NotificationManager => NotificationManager.FromContext(UIRuntime.CurrentActivity);

        public static void Configure(
            string name, string description, int iconResourceId, int transparentIconResourceId,
            Color transparentIconColor, bool sound, NotificationImportance importance = NotificationImportance.High,
            Action<Notification> onTapped = null)
        {
            Icon = new AndroidNotificationIcon(iconResourceId);
            TransparentIcon = new AndroidNotificationIcon(transparentIconResourceId);
            TransparentIconColor = transparentIconColor;
            OnTapped = onTapped;

            RegisterScheduledNotificationBroadcastReceiver();

            if (OS.IsAtLeast(BuildVersionCodes.O)) CreateChannel(name, description, sound, importance);

            if (onTapped is not null) UIRuntime.OnNewIntent.Handle(OnNewIntent);
        }

        static void RegisterScheduledNotificationBroadcastReceiver()
        {
            var receiver = new ScheduledNotificationsBroadcastReceiver();

            var filter = new IntentFilter { Priority = 100 };
            filter.AddAction(Intent.ActionScreenOn);
            filter.AddAction(Intent.ActionScreenOff);
            filter.AddAction(Intent.ActionBootCompleted);

            UIRuntime.CurrentActivity.RegisterReceiver(receiver, filter);

            App.Stopping += () =>
            {
                try { UIRuntime.CurrentActivity.UnregisterReceiver(receiver); }
                catch { }
            };
        }

        static void OnNewIntent(Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            if (extra.IsEmpty()) return;

            var notification = JsonConvert.DeserializeObject<AndroidLocalNotification>(extra);
            if (notification is null) return;

            OnTapped(new Notification
            {
                Title = notification.Title,
                Body = notification.Body,
                Id = notification.Id,
                NotifyTime = notification.NotifyTime,
                Parameters = notification.Parameters
            });
        }
    }
}