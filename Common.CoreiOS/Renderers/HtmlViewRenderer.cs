using System;
using System.ComponentModel;
using Common.Core.Controls;
using Common.Core.Tools;
using Common.CoreiOS.Renderers;
using CoreGraphics;
using Foundation;
using Plugin.Connectivity;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlView), typeof(HtmlViewRenderer))]
namespace Common.CoreiOS.Renderers
{
	public class HtmlViewRenderer : ViewRenderer<HtmlView, UITextView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<HtmlView> e)
		{
			base.OnElementChanged(e);
			if (Control == null)
			{
				var textView = new UITextView(CGRect.Empty) {
					Editable = false,
					DataDetectorTypes = UIDataDetectorType.All,
					TextContainer = {
						LineBreakMode = UILineBreakMode.WordWrap,
						HeightTracksTextView = true,
						WidthTracksTextView = true,
						MaximumNumberOfLines = nuint.MaxValue,
						LineFragmentPadding = 0
					},
					TextContainerInset = UIEdgeInsets.Zero,
					ScrollEnabled = false,
					BackgroundColor = UIColor.Clear
				};
				//textView.ShouldInteractWithUrl += (view, url, arg3) => {
				//	if (!UIApplication.SharedApplication.OpenUrl(url))
				//	{
				//		Device.OpenUri(new Uri(url.ToString()));
				//	}
				//	return false;
				//};
				SetNativeControl(textView);
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
			NSError error = null;
			var htmlString = new NSAttributedString(
				Element.Text,
				new NSAttributedStringDocumentAttributes
				{
					DocumentType = NSDocumentType.HTML,
					StringEncoding = NSStringEncoding.UTF8
				},
				ref error);
			Control.AttributedText = htmlString;
		}

		private void UpdateFont()
		{
			Control.Font = Element.Font.ToUIFont();
		}

		private void UpdateTextColor()
		{
			var textColor = Element.TextColor;
			Control.TextColor = textColor == Color.Default 
				? UIColor.Black 
				: textColor.ToUIColor();
		}
	}
}

public static class NSStringExtensions
{
	public static NSAttributedString AsAttributedString(this string input, NSDocumentType docType)
	{
		NSError err = null;
		var rtn = new NSAttributedString(input, new NSAttributedStringDocumentAttributes {
			DocumentType = docType,
			StringEncoding = NSStringEncoding.UTF8
		}, ref err);
		if (err == null)
		{
			return rtn;
		}
		throw new NSErrorException(err);
	}
}