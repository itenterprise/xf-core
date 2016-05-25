using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.Widget;
using Common.Core.Interfaces;
using Common.CoreDroid.Providers;
using Xamarin.Forms;

[assembly: Dependency(typeof(SkypeProvider))]
namespace Common.CoreDroid.Providers
{
	public class SkypeProvider : ISkypeProvider
	{
		public bool IsInstalled()
		{
			var packageMgr = Forms.Context.PackageManager;
			try
			{
				packageMgr.GetPackageInfo("com.skype.raider", PackageInfoFlags.Activities);
			}
			catch (PackageManager.NameNotFoundException e)
			{
				return false;
			}
			return true;
		}

		public void Open(string login, SkypeAction action)
		{
			// Make sure the Skype for Android client is installed.
			if (!IsInstalled())
			{
				goToMarket();
				return;
			}

			// Create the Intent from our Skype URI.
			var uri = Uri.Parse($"skype:{login}?{(action == SkypeAction.Call ? "call" : "chat")}");
			var intent = new Intent(Intent.ActionView, uri);

			// Restrict the Intent to being handled by the Skype for Android client only.
			intent.SetComponent(new ComponentName("com.skype.raider", "com.skype.raider.Main"));
			intent.SetFlags(ActivityFlags.NewTask);

			// Initiate the Intent. It should never fail because you've already established the
			// presence of its handler (although there is an extremely minute window where that
			// handler can go away).
			Forms.Context.StartActivity(intent);
		}

		private static void goToMarket()
		{
			var marketUri = Uri.Parse("market://details?id=com.skype.raider");
			var intent = new Intent(Intent.ActionView, marketUri);
			intent.SetFlags(ActivityFlags.NewTask);
			try
			{
				Forms.Context.StartActivity(intent);
			}
			catch (ActivityNotFoundException e)
			{
				Toast.MakeText(Forms.Context, Resource.String.cant_open_google_play, ToastLength.Short).Show();
			}
		}
	}
}