using System.Threading.Tasks;
using Common.Core.Helpers;
using Common.Core.View;
using Common.Core.ViewModel;
using Xamarin.Forms;

[assembly: Dependency(typeof(WelcomeStartPage))]
namespace Common.Core.View
{
	public partial class WelcomeStartPage
	{
		public WelcomeStartPage()
		{
			InitializeComponent();
			BarTextColor = Color.White;
			BackgroundColor = BarBackgroundColor;
			AuthenticateButton.Clicked += (sender, e) => ApplicationBase.Current.GotoAuthenticationPage();
		}

		protected async override void OnLoaded()
		{
			base.OnLoaded();
			await Task.Delay(300);
			await Label1Stack.ScaleTo(1, (uint)ApplicationBase.AnimationSpeed, Easing.SinIn);
			await Label2Stack.ScaleTo(1, (uint)ApplicationBase.AnimationSpeed, Easing.SinIn);
			await ButtonStack.ScaleTo(1, (uint)ApplicationBase.AnimationSpeed, Easing.SinIn);
			if (!Settings.IsAutenticated && ApplicationBase.IsNetworkRechable)
			{
				await ViewModel.TryLoginFromCurrentSessionOrSecureStorage();
				if (Settings.IsAutenticated)
				{
					ApplicationBase.Current.GotoMainPage();
				}
			}
		}
	}

	public class WelcomeStartPageXaml : BaseContentPageDI<WelcomeViewModel>
	{
	}
}