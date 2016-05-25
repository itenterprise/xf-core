using System;
using Common.Core.Interfaces;
using Common.CoreiOS.Providers;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SkypeProvider))]
namespace Common.CoreiOS.Providers
{
	public class SkypeProvider : ISkypeProvider
	{
		public bool IsInstalled()
		{
			return UIApplication.SharedApplication.CanOpenUrl(new NSUrl("skype:"));
		}

		public void Open(string login, SkypeAction action)
		{
			var skypeUri = $"skype:{login}?{(action == SkypeAction.Call ? "call" : "chat")}";
			if (!IsInstalled())
			{
				skypeUri = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
				? @"https://itunes.apple.com/us/app/skype-for-ipad/id442012681?mt=8"
				: @"https://itunes.apple.com/in/app/skype-for-iphone/id304878510?mt=8";
			}
			UIApplication.SharedApplication.OpenUrl(new NSUrl(skypeUri));
		}
	}
}
