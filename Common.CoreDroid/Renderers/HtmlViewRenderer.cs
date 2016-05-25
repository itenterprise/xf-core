using System.ComponentModel;
using Android.Text;
using Android.Text.Method;
using Android.Text.Util;
using Android.Widget;
using Common.Core.Controls;
using Common.CoreDroid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HtmlView), typeof(HtmlViewRenderer))]
namespace Common.CoreDroid.Renderers
{
	public class HtmlViewRenderer : Xamarin.Forms.Platform.Android.AppCompat.ViewRenderer<HtmlView, TextView>
	{
		protected override TextView CreateNativeControl()
		{
			return new TextView(Context)
			{
				MovementMethod = LinkMovementMethod.Instance
			};
		}

		protected override void OnElementChanged(ElementChangedEventArgs<HtmlView> e)
		{
			base.OnElementChanged(e);
			if (Element == null)
			{
				return;
			}
			var textView = new TextView(Context)
			{
				MovementMethod = LinkMovementMethod.Instance
			};
			SetNativeControl(textView);
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
			if (e.PropertyName == HtmlView.FontProperty.PropertyName)
			{
				UpdateFont();
			}
			if (e.PropertyName == HtmlView.TextProperty.PropertyName)
			{
				UpdateText();
			}
		}

		protected void UpdateText()
		{
			Control.TextFormatted = Html.FromHtml(Element.Text ?? string.Empty);
		}

		private void UpdateFont()
		{
			if (Element.Font != Font.Default)
			{
				Control.TextSize = Element.Font.ToScaledPixel();
			}
		}

		private void UpdateTextColor()
		{
			var textColor = Element.TextColor;
			if (textColor != Color.Default)
			{
				Control.SetTextColor(textColor.ToAndroid());
			}
		}
	}
}