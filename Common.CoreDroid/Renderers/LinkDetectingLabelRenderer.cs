using Android.Text.Util;
using Android.Util;
using Android.Widget;
using Common.Core.Controls;
using Common.CoreDroid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinkDetectingLabel), typeof(LinkDetectingLabelRenderer))]

namespace Common.CoreDroid.Renderers
{
	public class LinkDetectingLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var detectingLabel = (LinkDetectingLabel)Element;
			if (detectingLabel == null) return;

			// Создание нативного контрола
			var textView = new TextView(Forms.Context);
			textView.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
			textView.SetTextColor(detectingLabel.TextColor.ToAndroid());
			

			// Выбор типа ссылок
			MatchOptions? linkType;
			switch (detectingLabel.DataLinkType)
			{
				case LinkDetectingLabel.LinkType.Url:
					linkType = MatchOptions.WebUrls;
					break;
				case LinkDetectingLabel.LinkType.Email:
					linkType = MatchOptions.EmailAddresses;
					break;
				case LinkDetectingLabel.LinkType.Phone:
					linkType = MatchOptions.PhoneNumbers;
					break;
				case LinkDetectingLabel.LinkType.Map:
					linkType = MatchOptions.MapAddresses;
					break;
				case LinkDetectingLabel.LinkType.All:
					linkType = MatchOptions.All;
					break;
				default:
					linkType = null;
					break;
			}
			if (linkType != null)
			{
				textView.AutoLinkMask = linkType.Value;
			}
			
			// Задавать текст только после вибора типа ссылки
			textView.Text = detectingLabel.Text;
			textView.SetTextSize(ComplexUnitType.Dip, (float)detectingLabel.FontSize);

			// Переопределение Xamarin Forms Label и замена на нативный контрол
			SetNativeControl(textView);
		}
	}
}