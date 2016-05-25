using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Common.Core.Controls;
using Common.CoreUWP.Providers;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(HtmlView), typeof(HtmlViewRenderer))]
namespace Common.CoreUWP.Providers
{
	public class HtmlViewRenderer : ViewRenderer<HtmlView, TextBlock>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<HtmlView> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				var textBox = new TextBlock {
				};
				//todo:?
				SetNativeControl(textBox);
			}
			if (e.NewElement == null)
			{
				return;
			}
			UpdateText();
			UpdateTextColor();
			UpdateFont();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (Control == null || Element == null)
			{
				return;
			}
			if (e.PropertyName == HtmlView.TextProperty.PropertyName)
			{
				UpdateText();
				UpdateFont();
			}
			if (e.PropertyName == HtmlView.FontProperty.PropertyName)
			{
				UpdateFont();
			}
			if (e.PropertyName == HtmlView.FontFamilyProperty.PropertyName)
			{
				UpdateFont();
			}
			if (e.PropertyName == HtmlView.TextColorProperty.PropertyName)
			{
				UpdateTextColor();
			}
		}

		private void UpdateText()
		{
			if (Control.Text == Element.Text)
				return;
			Control.Text = Element.Text;
		}

		private void UpdateFont()
		{
			//todo:
		}

		private void UpdateTextColor()
		{
			//todo:
		}
	}
}