using System;
using Common.Core.Interfaces;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class HtmlView : Xamarin.Forms.View
	{
		/// <summary>
		/// The text property.
		/// </summary>
		public static readonly BindableProperty TextProperty = BindableProperty.Create<HtmlView, string>(
			p => p.Text, string.Empty, BindingMode.TwoWay, null, TextValueChanged);

		public string Text
		{
			get { return Convert.ToString(GetValue(TextProperty)); }
			set
			{
				SetValue(TextProperty, value);
			}
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set
			{
				SetValue(TextColorProperty, value);
			}
		}

		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
			"TextColor", 
			typeof(Color), 
			typeof(Editor), 
			Color.Default);

		public static readonly BindableProperty FontProperty =  BindableProperty.Create(
			"Font", 
			typeof(Font), 
			typeof(HtmlView), 
			Font.SystemFontOfSize(Device.OS == TargetPlatform.iOS 
				? NamedSize.Small
				: NamedSize.Default));

		public Font Font
		{
			get
			{
				return (Font)GetValue(FontProperty);
			}
			set
			{
				SetValue(FontProperty, value);
			}
		}

		public static readonly BindableProperty FontFamilyProperty =
			BindableProperty.Create("FontFamily", typeof(string), typeof(HtmlView), null);

		public string FontFamily
		{
			get
			{
				return (string)GetValue(FontFamilyProperty);
			}
			set
			{
				SetValue(FontFamilyProperty, value);
			}
		}

		private static void TextValueChanged(BindableObject obj, string oldTextValue, string newTextValue)
		{
			var control = obj as HtmlView;
			if (control != null)
			{
				var linkReplacer = DependencyService.Get<ILinkReplacer>();
				if (linkReplacer != null)
				{
					newTextValue = linkReplacer.Exec(newTextValue);
				}
				control.Text = newTextValue;
			}
		}
	}
}