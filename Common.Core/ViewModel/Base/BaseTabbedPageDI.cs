using Xamarin.Forms;

namespace Common.Core.ViewModel
{
	public class BaseTabbedPageDI<T> : BaseTabbedPage<T> where T : BaseViewModel, new()
	{
		protected override T CreateViewModel()
		{
			return DependencyService.Get<T>(DependencyFetchTarget.NewInstance);
		}
	}
}