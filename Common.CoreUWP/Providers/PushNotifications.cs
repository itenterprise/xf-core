using Common.Core;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.CoreUWP.Providers;
using Xamarin.Forms;

[assembly: Dependency(typeof(PushNotifications))]
namespace Common.CoreUWP.Providers
{
	public class PushNotifications : IPushNotifications
	{
		public void SubscribePushNotifications(bool silentMode = false)
		{
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
}