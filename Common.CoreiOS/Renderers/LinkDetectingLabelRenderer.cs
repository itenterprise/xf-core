using System;
using System.Collections.Generic;
using System.Text;
using Common.Core.Controls;
using Common.CoreiOS.Renderers;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LinkDetectingLabel), typeof(LinkDetectingLabelRenderer))]

namespace Common.CoreiOS.Renderers
{
	public class LinkDetectingLabelRenderer : ViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			var detectingLabel = (LinkDetectingLabel)Element;
			if (detectingLabel == null) return;

			// Создание нативного контрола
			var textView = new UITextView(new CGRect(0, 0, detectingLabel.Width, detectingLabel.Height)) {
				Text = detectingLabel.Text,
				Font = UIFont.SystemFontOfSize((float)detectingLabel.FontSize),
				BackgroundColor = UIColor.Clear,
				Editable = false,
				Selectable = true
			};

			// Выбор типа ссылок
			UIDataDetectorType linkType;
			switch (((LinkDetectingLabel)e.NewElement).DataLinkType)
			{
				case LinkDetectingLabel.LinkType.Url:
				case LinkDetectingLabel.LinkType.Email:
					linkType = UIDataDetectorType.Link;
					break;
				case LinkDetectingLabel.LinkType.Phone:
					linkType = UIDataDetectorType.PhoneNumber;
					break;
				case LinkDetectingLabel.LinkType.Map:
					linkType = UIDataDetectorType.Address;
					break;
				case LinkDetectingLabel.LinkType.All:
					linkType = UIDataDetectorType.All;
					break;
				default:
					linkType = UIDataDetectorType.None;
					break;
			}
			textView.DataDetectorTypes = linkType;

			// Переопределение Xamarin Forms Label и замена на нативный контрол
			SetNativeControl(textView);
		}
	}
}
