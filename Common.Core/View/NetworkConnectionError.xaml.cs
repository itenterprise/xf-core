using Common.Core.ViewModel;

namespace Common.Core.View
{
	public partial class NetworkConnectionError
	{
		public NetworkConnectionError()
		{
			InitializeComponent();
			ViewModel.ErrorMessage = "Отсутствует подключение к интернету! Повторите попытку позднее.";
		}
	}

	public class NetworkConnectionErrorXaml : BaseContentPage<ErrorViewModel>
	{
	}
}