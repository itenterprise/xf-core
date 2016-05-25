using Common.Core.ViewModel;

namespace Common.Core.View
{
	public partial class ErrorPage
	{
		public ErrorPage(string errorMessage)
		{
			InitializeComponent();
			ViewModel.ErrorMessage = errorMessage;
		}
	}

	public class ErrorPageXaml : BaseContentPage<ErrorViewModel>
	{
	}
}