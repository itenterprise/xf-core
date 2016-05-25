using System;
using System.Net;
using Acr.UserDialogs;
using Android.Content;
using Android.OS;
using Common.Core;
using Common.Core.Interfaces;
using FFImageLoading.Forms.Droid;
using Newtonsoft.Json;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Common.CoreDroid
{
	public abstract class MainActivityBase : FormsAppCompatActivity
	{
		public static MainActivityBase Current { get; set; }

		public abstract ApplicationBase CreateApplication();

		public static bool IsRunning { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				try
				{
					var ex = ((Exception)e.ExceptionObject).GetBaseException();
					Console.WriteLine("**MAIN ACTIVITY EXCEPTION**\n\n" + ex);
				}
				catch (Exception e1)
				{
					ApplicationBase.TrackException(e1);
				}
			};
			try
			{
				var app = ApplicationBase.Current;
				base.OnCreate(bundle);

				Forms.Init(this, bundle);
				CachedImageRenderer.Init();
				ZXing.Net.Mobile.Forms.Android.Platform.Init();
				UserDialogs.Init(CrossCurrentActivity.Current.Activity);
#if DEBUG
				ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
#endif
				LoadApplication(app ?? CreateApplication());
				Current = this;
				OnNewIntent(Intent);
			}
			catch (Exception e2)
			{
				ApplicationBase.TrackException(e2);
			}
		}

		protected override void OnNewIntent(Intent intent)
		{
			base.OnNewIntent(intent);
			processNotificationPayload(intent);
		}

		protected override void OnPause()
		{
			IsRunning = false;
			base.OnPause();
		}

		protected override void OnResume()
		{
			IsRunning = true;
			processNotificationPayload(Intent);
			base.OnResume();
		}

		private void processNotificationPayload(Intent intent)
		{
			if (intent == null || intent.HasExtra("used"))
			{
				return;
			}
			intent.PutExtra("used", true);
			var payloadString = intent.GetStringExtra("payload");
			if (string.IsNullOrEmpty(payloadString))
			{
				return;
			}
			var payload = JsonConvert.DeserializeObject<NotificationPayload>(payloadString);
			ApplicationBase.Current.OnIncomingPayload(payload);
		}
	}
}