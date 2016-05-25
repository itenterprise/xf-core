using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms;

namespace Common.Core.Tools
{
	/// <summary>
	/// Коллекция, которая умеет сообщать о своем изменении один раз после массового добавления элементов
	/// Значительно повышает отзывчивость интерфейса при использовании биндинга
	/// </summary>
	public class ObservableCollectionFast<T> : ObservableCollection<T>
		where T : class
	{
		public ObservableCollectionFast()
		{
		}

		public ObservableCollectionFast(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public ObservableCollectionFast(List<T> list)
			: base(list)
		{
		}

		public void AddRangeTop(IList range, bool reverse = false)
		{
			for (var i = 0; i < range.Count; i++)
			{
				Items.Insert(reverse ? 0 : i, range[i] as T);
			}

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, range, 0));
		}

		public void AddRangeBottom(IList range, bool reverse = false)
		{
			var startIndex = Count;
			foreach (var item in range)
			{
				if (reverse)
				{
					Items.Insert(Count, item as T);
				}
				else
				{
					Items.Add(item as T);
				}
			}

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(Device.OS == TargetPlatform.iOS
				? new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
				: new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, range, startIndex));
		}

		public void Reset(IList range)
		{
			Items.Clear();
			foreach (var item in range)
			{
				Items.Insert(Count, item as T);
			}

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}