using Common.Core.Controls;
using Common.CoreiOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace Common.CoreiOS.Renderers
{
	public class ExtendedButtonRenderer : ButtonRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				return;
			}
			Control.ContentEdgeInsets = new UIEdgeInsets(0, 5, 0, 5);
		}
	}
}