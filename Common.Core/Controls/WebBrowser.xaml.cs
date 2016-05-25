using System;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public partial class WebBrowser
	{
		/// <summary>
		/// The placeholder property.
		/// </summary>
		public static readonly BindableProperty LinkProperty =
			BindableProperty.Create<WebBrowser, string>(p => p.Link, string.Empty, BindingMode.TwoWay, null, linkChanged);


		/// <summary>
		/// Gets or sets the placeholder.
		/// </summary>
		/// <value>The placeholder.</value>
		public string Link
		{
			get { return (string)GetValue(LinkProperty); }
			set { SetValue(LinkProperty, value); }
		}

		/// <summary>
		/// The placeholder property.
		/// </summary>
		public static readonly BindableProperty HtmlProperty =
			BindableProperty.Create<WebBrowser, string>(p => p.Html, string.Empty, BindingMode.TwoWay, null, htmlChanged);


		/// <summary>
		/// Gets or sets the placeholder.
		/// </summary>
		/// <value>The placeholder.</value>
		public string Html
		{
			get { return (string)GetValue(HtmlProperty); }
			set { SetValue(HtmlProperty, value); }
		}

		public WebBrowser()
		{
			InitializeComponent();
		}

		private static void linkChanged(BindableObject bindable, string oldValue, string newValue)
		{
			var browser = bindable as WebBrowser;
			if (browser != null)
			{
				browser.WebView.Source = new UrlWebViewSource {
					Url = newValue
				};
			}
		}
		private static void htmlChanged(BindableObject bindable, string oldValue, string newValue)
		{
			var browser = bindable as WebBrowser;
			if (browser != null)
			{
				browser.WebView.Source = new HtmlWebViewSource {
					Html = newValue
				};
			}
		}

		public void GoBack()
		{
			//check to see if there is anywhere to go back to
			if (WebView.CanGoBack)
			{
				WebView.GoBack();
			}
			else
			{
				//if not, leave the view
				Navigation.PopAsync();
			}
		}

		public void GoForward()
		{
			if (WebView.CanGoForward)
			{
				WebView.GoForward();
			}
		}
	}
}