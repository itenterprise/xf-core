using System;
using System.Collections.Generic;
using Common.Core.ViewModel;

namespace Common.Core.Factory
{
	public class ViewModelFactoryBase<TKey, TViewModel> where TViewModel : BaseViewModel
	{
		protected readonly Dictionary<TKey, TViewModel> Cache = new Dictionary<TKey, TViewModel>();

		/// <summary>
		/// Получить view model для хаба
		/// </summary>
		/// <param name="key">Идентификатор хаба</param>
		/// <returns></returns>
		public TViewModel GetViewModel(TKey key)
		{
			return GetViewModel(key, null);
		}

		/// <summary>
		/// Получить view model для хаба
		/// </summary>
		/// <param name="key">Идентификатор хаба</param>
		/// <param name="activator">Переопределение функции создания ViewModel</param>
		public TViewModel GetViewModel(TKey key, Func<TViewModel> activator = null)
		{
			if (Cache.ContainsKey(key))
			{
				return Cache[key];
			}
			activator = activator ?? (() => (TViewModel)Activator.CreateInstance(typeof(TViewModel), key));
			var viewModel = activator.Invoke();
			Cache[key] = viewModel;
			return viewModel;
		}
	}
}