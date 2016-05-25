using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class HideableToolbarItem : ToolbarItem
	{
		public HideableToolbarItem()
		{
			InitVisibility();
		}

		private async void InitVisibility()
		{
			await Task.Delay(100);
			onIsVisibleChanged(this, false, IsVisible);
		}

		public ContentPage Parent { set; get; }

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static BindableProperty IsVisibleProperty =
			BindableProperty.Create<HideableToolbarItem, bool>(o => o.IsVisible, false, propertyChanged: onIsVisibleChanged);

		private static void onIsVisibleChanged(BindableObject bindable, bool oldvalue, bool newvalue)
		{
			var item = bindable as HideableToolbarItem;

			if (item == null || item.Parent == null)
			{
				return;
			}
			var items = item.Parent.ToolbarItems;

			if (newvalue && !items.Contains(item))
			{
				items.Add(item);
			}
			else if (!newvalue && items.Contains(item))
			{
				items.Remove(item);
			}
		}
	}
}