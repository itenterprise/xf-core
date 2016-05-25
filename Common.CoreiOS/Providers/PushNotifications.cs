using Common.Core;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.CoreiOS.Providers;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PushNotifications))]
namespace Common.CoreiOS.Providers
{
	public class PushNotifications : IPushNotifications
	{
		public void SubscribePushNotifications(bool silentMode = false)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var notificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge;
				if (!silentMode)
				{
					notificationTypes = notificationTypes | UIUserNotificationType.Sound;
				}
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
					notificationTypes,
					new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else
			{
				var notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge;
				if (!silentMode)
				{
					notificationTypes = notificationTypes | UIRemoteNotificationType.Sound;
				}
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
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

		public bool IsRegistered
		{
			get
			{
				return UIApplication.SharedApplication.IsRegisteredForRemoteNotifications;
			}
		}
	}
}