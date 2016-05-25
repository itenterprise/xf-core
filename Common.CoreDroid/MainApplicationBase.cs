using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Common.Core.Interfaces;
using Plugin.CurrentActivity;

namespace Common.CoreDroid
{
	public class MainApplicationBase : Application, Application.IActivityLifecycleCallbacks
	{
		public static MainApplicationBase Current { get; set; }

		public static string NotificationSenderId { get; set; }

		public MainApplicationBase(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
			Current = this;
			RegisterActivityLifecycleCallbacks(this);
			//A great place to initialize Xamarin.Insights and Dependency Services!
		}

		public virtual NotificationParameters GetNotificationParameters(NotificationPayload notificationPayload)
		{
			var notification = new NotificationParameters();
			if (notificationPayload == null || notificationPayload.Payload == null)
			{
				return notification;
			}
			var payload = notificationPayload.Payload;
			object headerObj;
			if (payload.TryGetValue("HEADER", out headerObj))
			{
				notification.Title = (headerObj as string) ?? string.Empty;
			}
			object notify;
			if (payload.TryGetValue("NOTIFY", out notify))
			{
				notification.Visible = false;
			}
			return notification;
		}

		public override void OnTerminate()
		{
			base.OnTerminate();
			UnregisterActivityLifecycleCallbacks(this);
		}

		public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}

		public void OnActivityDestroyed(Activity activity)
		{
		}

		public void OnActivityPaused(Activity activity)
		{
		}

		public void OnActivityResumed(Activity activity)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}

		public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
		}

		public void OnActivityStarted(Activity activity)
		{
			CrossCurrentActivity.Current.Activity = activity;
		}

		public void OnActivityStopped(Activity activity)
		{
		}
	}
}