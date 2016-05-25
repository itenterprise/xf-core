using Acr.UserDialogs;
using Common.Core.Interfaces;
using Common.CoreUWP.Providers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HUDProvider))]
namespace Common.CoreUWP.Providers
{
	public class HUDProvider : IHUDProvider
	{
		public void DisplayProgress(string message)
		{
			UserDialogs.Instance.ShowLoading(
				string.IsNullOrWhiteSpace(message) 
					? null 
					: message, 
				MaskType.Black);
		}

		public void DisplaySuccess(string message)
		{
			UserDialogs.Instance.ShowSuccess(message);
		}

		public void DisplayError(string message)
		{
			UserDialogs.Instance.ShowError(message);
		}

		public void Dismiss()
		{
			UserDialogs.Instance.HideLoading();
		}
	}
}
