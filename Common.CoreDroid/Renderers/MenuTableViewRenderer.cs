using Android.Content;
using Android.Views;
using Android.Widget;
using Common.Core.Controls;
using Common.CoreDroid.Renderers;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(MenuTableView), typeof(MenuTableViewRenderer))]
namespace Common.CoreDroid.Renderers
{
	public class MenuTableViewRenderer : TableViewRenderer
	{
		protected override TableViewModelRenderer GetModelRenderer(global::Android.Widget.ListView listView, TableView view)
		{
			return new CustomTableViewModelRenderer(this.Context, listView, view);
		}
		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				return;
			}
			Control.DividerHeight = 0;
			Control.SetHeaderDividersEnabled(false);
			Control.SetFooterDividersEnabled(false);
		}
	}

	public class CustomTableViewModelRenderer : TableViewModelRenderer
	{

		public CustomTableViewModelRenderer(Context Context, global::Android.Widget.ListView ListView, TableView View)
			: base(Context, ListView, View)
		{ }
		public override global::Android.Views.View GetView(int position, global::Android.Views.View convertView, ViewGroup parent)
		{
			var view = base.GetView(position, convertView, parent);

			var element = this.GetCellForPosition(position);

			if (element.GetType() == typeof(TextCell))
			{
				try
				{
					var layout = view as LinearLayout;
					if (layout != null)
					{
						var linearLayout = layout.GetChildAt(0) as LinearLayout;
						if (linearLayout != null)
						{
							var linearLayout1 = linearLayout.GetChildAt(1) as LinearLayout;
							if (linearLayout1 != null)
							{
								var text = (linearLayout1.GetChildAt(0) as TextView);
								if (text != null)
								{
									text.SetTextColor(Color.Rgb(109, 109, 114));
								}
							}
						}
						var divider = layout.GetChildAt(1);
						if (divider != null)
						{
							divider.SetBackgroundColor(Color.Transparent);
						}
					}
				}
				catch
				{
				}
			}

			return view;
		}
	}
}