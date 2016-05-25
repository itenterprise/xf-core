using System;
using System.Diagnostics;
using System.Net;
using AudioToolbox;
using Common.Core;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.Tools;
using Common.CoreiOS.Renderers;
using FFImageLoading.Forms.Touch;
using Foundation;
using Newtonsoft.Json;
using UIKit;
using Xamarin.Forms;

namespace Common.CoreiOS
{
	public abstract class App : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public abstract UIColor BarTintColor { get; }

		public abstract UIColor BarTitleColor { get; }

		public abstract UIColor TintColor { get; }

		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			UINavigationBar.Appearance.BarTintColor = BarTintColor; //bar background
			UINavigationBar.Appearance.TintColor = TintColor; //Tint color of button items
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				Font = UIFont.SystemFontOfSize(20),
				TextColor = BarTitleColor
			});

#if DEBUG
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
#endif
			Forms.Init();
			CachedImageRenderer.Init();
			KeyboardOverlapRenderer.Init();
			ZXing.Net.Mobile.Forms.iOS.Platform.Init();
			LoadApplication(CreateApplication());
			// Вылет на iOS 7
			//var notificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge;
			//if (Settings.Sound)
			//{
			//	notificationTypes  = notificationTypes | UIUserNotificationType.Sound;
			//}
			//uiApplication.RegisterUserNotificationSettings(UIUserNotificationSettings.GetSettingsForTypes(notificationTypes, null));
			ProcessPendingPayload(launchOptions);
			return base.FinishedLaunching(uiApplication, launchOptions);
		}

		public abstract ApplicationBase CreateApplication();

		protected static void ProcessPendingPayload(NSDictionary userInfo)
		{
			if (userInfo == null)
			{
				return;
			}
			NSObject additional;
			if (userInfo.TryGetValue(new NSString("additional"), out additional))
			{
				var additionalString = additional.ToString();
				if (!string.IsNullOrEmpty(additionalString))
				{
					var notificationParams = JsonConvert.DeserializeObject<IncomingPushNotificationEventArgs>(additional.ToString());
					if (notificationParams != null && notificationParams.Payload != null)
					{
						var payloadValue = JsonConvert.SerializeObject(notificationParams.Payload);
						var notificationPayload = JsonConvert.DeserializeObject<NotificationPayload>(payloadValue);
						notificationPayload.NotificationInteractionType = NotificationInteractionType.NotificationClicked;
						ApplicationBase.Current.OnIncomingPayload(notificationPayload);
					}
				}
			}
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			var registrationId = deviceToken.Description.Trim('<', '>').Replace(" ", "");

			WebServiceHelper.ExecuteNonQuery("REGDEVICE", new
			{
				appid = ApplicationBase.Current.AppId,
				deviceid = registrationId,
				prevdeviceid = Settings.NotificationRegistrationId ?? string.Empty,
				mobileos = "IOS"
			});
			Settings.NotificationRegistrationId = registrationId;
		}

		public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
		{
			Debug.WriteLine("FailedToRegisterForRemoteNotifications called");
			Debug.WriteLine(error);
		}
		
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			var alertText = string.Empty;
			string headerText;
			NSObject aps;
			if (userInfo.TryGetValue(new NSString("aps"), out aps))
			{
				var apsHash = aps as NSDictionary;
				if (apsHash != null)
				{
					NSObject alert;
					if (apsHash.TryGetValue(new NSString("alert"), out alert))
					{
						if (application.ApplicationState == UIApplicationState.Active)
						{
							alertText = alert.ToString();
						}
					}
				}
			}
			parseAdditionalInfo(application, userInfo, out headerText);


			if (!string.IsNullOrWhiteSpace(alertText))
			{
				var localNotification = new UILocalNotification();
				if (!string.IsNullOrEmpty(headerText))
				{
					localNotification.AlertTitle = headerText;
				}
				localNotification.FireDate = (NSDate)DateTime.Now;
				localNotification.AlertBody = alertText;
				localNotification.UserInfo = userInfo;
				UIApplication.SharedApplication.ScheduleLocalNotification(localNotification);
				if (Settings.SignalAboutNotifications)
				{
					SystemSound.Vibrate.PlayAlertSound(() => "Новое уведомление".ToToast(headerText));
				}
				else
				{
					"Новое уведомление".ToToast(headerText);
				}
			}
			completionHandler(UIBackgroundFetchResult.NewData);
		}

		private void parseAdditionalInfo(UIApplication application, NSDictionary userInfo, out string headerText)
		{
			headerText = null;
			if (userInfo == null)
			{
				return;
			}
			NSObject additional;
			if (userInfo.TryGetValue(new NSString("additional"), out additional))
			{
				var additionalString = additional.ToString();
				if (!string.IsNullOrEmpty(additionalString))
				{
					var notificationParams = JsonConvert.DeserializeObject<IncomingPushNotificationEventArgs>(additional.ToString());
					if (notificationParams != null && notificationParams.Payload != null)
					{
						var payloadValue = JsonConvert.SerializeObject(notificationParams.Payload);
						var payload = JsonConvert.DeserializeObject<NotificationPayload>(payloadValue);
						object headerTextObj;
						if (payload.Payload.TryGetValue("HEADER", out headerTextObj))
						{
							headerText = headerTextObj as string;
						}
						payload.NotificationInteractionType = application.ApplicationState == UIApplicationState.Active
							? NotificationInteractionType.NotificationInApp
							: NotificationInteractionType.NotificationClicked;
						MessagingCenter.Send(ApplicationBase.Current, Messages.IncomingPayloadReceivedInternal, payload);
					}
					/*
					var badgeValue = additionalHash.ObjectForKey(new NSString("badge"));
					if (badgeValue != null)
					{
						int count;
						if (int.TryParse(new NSString(badgeValue.ToString()), out count))
						{
							UIApplication.SharedApplication.ApplicationIconBadgeNumber = count;
						}
					}
					*/
				}
			}
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			string headerText;
			parseAdditionalInfo(application, notification.UserInfo, out headerText);
		}

		//iOS > 9.0
		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			if (string.IsNullOrWhiteSpace(url.Scheme))
			{
				return false;
			}
			MessagingCenter.Send(ApplicationBase.Current, Messages.IncomingUrl, new Uri(HttpUtility.UrlDecode(url.AbsoluteString)));
			return true;
		}

		//iOS < 9.0
		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			if (string.IsNullOrWhiteSpace(url.Scheme))
			{
				return false;
			}
			MessagingCenter.Send(ApplicationBase.Current, Messages.IncomingUrl, new Uri(HttpUtility.UrlDecode(url.AbsoluteString)));
			return true;
		}
	}
}