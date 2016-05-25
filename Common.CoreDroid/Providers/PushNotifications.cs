using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Common.Core;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.Tools;
using Common.CoreDroid.Providers;
using Newtonsoft.Json;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using NotificationCompat = Android.Support.V7.App.NotificationCompat;

[assembly: UsesPermission(Android.Manifest.Permission.ReceiveBootCompleted)]
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: Dependency(typeof(PushNotifications))]

namespace Common.CoreDroid.Providers
{
	public class PushNotifications : IPushNotifications
	{
		public void SubscribePushNotifications(bool silentMode = false)
		{
			if (string.IsNullOrEmpty(MainApplicationBase.NotificationSenderId))
			{
				return;
			}
			try
			{
				GcmClient.CheckDevice(Forms.Context);
				GcmClient.CheckManifest(Forms.Context);
				GcmClient.Register(Forms.Context, MainApplicationBase.NotificationSenderId);
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public void UnSubscribePushNotifications()
		{
			WebServiceHelper.ExecuteNonQuery("UNREGUSER", new
			{
				appid = ApplicationBase.Current.AppId,
				deviceid = Settings.NotificationRegistrationId
			});
			Settings.NotificationRegistrationId = null;
		}
	}

	[BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS),
		IntentFilter(new[] { Intent.ActionBootCompleted }),
		IntentFilter(new[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new[] { "@PACKAGE_NAME@" }),
		IntentFilter(new[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new[] { "@PACKAGE_NAME@" }),
		IntentFilter(new[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		public override void OnReceive(Context context, Intent intent)
		{
			var pm = PowerManager.FromContext(context);
			var sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "GCM Broadcast Reciever Tag");
			sWakeLock.Acquire();

			if (!HandlePushNotification(context, intent))
			{
				base.OnReceive(context, intent);
			}

			sWakeLock.Release();
		}

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		/// <param name="intent"></param>
		/// <returns></returns>
		internal static bool HandlePushNotification(Context context, Intent intent)
		{
			if (!intent.Extras.ContainsKey("alert"))
			{
				return false;
			}
			var message = intent.Extras.Get("alert").ToString();

			var additionalParameters = intent.Extras.ContainsKey("additional")
				? intent.Extras.Get("additional").ToString()
				: null;
			string payloadValue = null;
			NotificationPayload payload = null;
			NotificationParameters notificationParameters = null;
			if (additionalParameters != null)
			{
				var param = JsonConvert.DeserializeObject<IncomingPushNotificationEventArgs>(additionalParameters);
				payloadValue = JsonConvert.SerializeObject(param.Payload);
				payload = JsonConvert.DeserializeObject<NotificationPayload>(payloadValue);
				var currentApplication = MainApplicationBase.Current;
				notificationParameters = currentApplication?.GetNotificationParameters(payload);
			}
			notificationParameters = notificationParameters ?? new NotificationParameters();
			var activityIntent = new Intent(context, notificationParameters.ActivityType);
			if (payloadValue != null)
			{
				activityIntent.PutExtra("payload", payloadValue);
			}
			activityIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
			var notificationId = Settings.LastNotificationId++;
			var pendingIntent = PendingIntent.GetActivity(context, notificationId, activityIntent, PendingIntentFlags.UpdateCurrent);

			var notificationBuilder = new NotificationCompat.Builder(context);
			notificationBuilder.SetSmallIcon(notificationParameters.Icon);
			if (notificationParameters.MakeLight)
			{
				notificationBuilder.SetLights(Android.Graphics.Color.Blue, 300, 1000);
			}
			notificationBuilder.SetContentIntent(pendingIntent);
			notificationBuilder.SetContentTitle(notificationParameters.Title);
			notificationBuilder.SetTicker(message);
			notificationBuilder.SetContentText(message);
			notificationBuilder.SetAutoCancel(true);
			if (notificationParameters.Vibrate)
			{
				notificationBuilder.SetVibrate(new long[] {
					200,
					200,
					100,
				});
			}

			var notificationManager = NotificationManagerCompat.From(context);
			if (notificationParameters.Visible)
			{
				if (message.Length < 30)
				{
					notificationManager.Notify(notificationId, notificationBuilder.Build());
				}
				else
				{
					var notification = new Android.Support.V4.App.NotificationCompat.BigTextStyle(notificationBuilder)
						.SetBigContentTitle(notificationParameters.Title)
						.BigText(message)
						.Build();
					notificationManager.Notify(notificationId, notification);
				}
			}

			if (!MainActivityBase.IsRunning)
			{
				return true;
			}
			try
			{
				if (payload != null)
				{
					payload.NotificationInteractionType = MainActivityBase.Current == null || MainActivityBase.Current.IsDestroyed
							? NotificationInteractionType.NotificationClicked
							: NotificationInteractionType.NotificationInApp;
					Device.BeginInvokeOnMainThread(() => {
						MessagingCenter.Send(ApplicationBase.Current, Messages.IncomingPayloadReceivedInternal, payload);
					});
				}
			}
			catch (Exception)
			{
				// ignored
			}
			return true;
		}
	}

	[Service]
	public class PushHandlerService : GcmServiceBase
	{
		public PushHandlerService() : base(MainApplicationBase.NotificationSenderId)
		{
		}

		protected override void OnRegistered(Context context, string registrationId)
		{
			try
			{
				Console.WriteLine(registrationId);

				WebServiceHelper.ExecuteNonQuery("REGDEVICE", new
				{
					appid = ApplicationBase.Current.AppId,
					deviceid = registrationId,
					prevdeviceid = Settings.NotificationRegistrationId ?? string.Empty,
					mobileos = "ANDROID"
				});
				Settings.NotificationRegistrationId = registrationId;
			}
			catch
			{
				// ignored
			}
		}

		protected override void OnMessage(Context context, Intent intent)
		{
			GcmBroadcastReceiver.HandlePushNotification(context, intent);
		}

		protected override void OnUnRegistered(Context context, string registrationId)
		{
			//Receive notice that the app no longer wants notifications
		}

		protected override void OnError(Context context, string errorId)
		{
			//Some more serious error happened
		}
	}
}