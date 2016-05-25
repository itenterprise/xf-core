using Xamarin.Forms;

namespace Common.Core.ViewModel
{
	/// <summary>
	/// Базовый класс страницы, модель которой переопределяется
	/// </summary>
	public class BaseContentPageDI<T> : BaseContentPage<T> where T : BaseViewModel, new()
	{
		protected override T CreateViewModel()
		{
			return DependencyService.Get<T>(DependencyFetchTarget.NewInstance);
		}
	}
}