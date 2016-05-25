using Common.Core.ViewModel;
using Xamarin.Forms;

namespace Common.Core.View
{
	public partial class AuthenticationPage
	{
		public AuthenticationPage()
		{
			InitializeComponent();
			Children.Add(new LoginPage(ViewModel) {
				Title = Properties.Resources.Enter,
				Icon = Device.OnPlatform("profile_menu.png", "", "")
			});
			if (ApplicationBase.Current.AllowToChangeUrl)
			{
				Children.Add(new UrlSettingsPage(ViewModel) {
					Title = Properties.Resources.Properties,
					Icon = Device.OnPlatform("configuration_menu.png", "", "")
				});
			}
		}
	}

	public class AuthenticationPageXaml : BaseTabbedPage<AuthenticationViewModel>
	{
	}
}