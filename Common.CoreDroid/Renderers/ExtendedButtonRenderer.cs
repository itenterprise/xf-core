using Common.Core.Controls;
using Common.CoreDroid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace Common.CoreDroid.Renderers
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
			//todo add inset
			Control.SetPadding(Control.PaddingLeft, 0, Control.PaddingRight, 0);
			var view = (ExtendedButton)this.Element;
			var nativeButton = (global::Android.Widget.Button)this.Control;
			//nativeButton.SetPadding((int)view.Padding.Left, (int)view.Padding.Top, (int)view.Padding.Right, (int)view.Padding.Bottom);
		}
	}
}