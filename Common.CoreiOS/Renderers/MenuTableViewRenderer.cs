using Common.Core.Controls;
using Common.CoreiOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MenuTableView), typeof(MenuTableViewRenderer))]
namespace Common.CoreiOS.Renderers
{
	public class MenuTableViewRenderer : TableViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				return;
			}
			Control.SeparatorStyle = UITableViewCellSeparatorStyle.None;
		}
	}
}