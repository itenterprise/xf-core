using Common.Core.ViewModel;

namespace Common.Core.View
{
	public partial class LoginPage
	{
		public LoginPage(AuthenticationViewModel viewModel) : base(viewModel)
		{
			InitializeComponent();
		}
	}

	public class LoginPageXaml : BaseContentPage<AuthenticationViewModel>
	{
		public LoginPageXaml(AuthenticationViewModel viewModel) : base(viewModel)
		{
		}
	}
}