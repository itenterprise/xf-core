using Common.Core.ViewModel;

namespace Common.Core.View
{
	public partial class UrlSettingsPage
	{
		public UrlSettingsPage()
		{
			InitializeComponent();
		}

		public UrlSettingsPage(UrlSettingsViewModel viewModel) : base(viewModel)
		{
			InitializeComponent();
		}
	}

	public class UrlSettingsPageXaml : BaseContentPage<UrlSettingsViewModel>
	{
		public UrlSettingsPageXaml()
		{
		}

		public UrlSettingsPageXaml(UrlSettingsViewModel viewModel) : base(viewModel)
		{
		}
	}
}