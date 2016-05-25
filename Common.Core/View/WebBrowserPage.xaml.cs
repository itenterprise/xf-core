using ToolbarItem = Xamarin.Forms.ToolbarItem;

namespace Common.Core.View
{
	public partial class WebBrowserPage
	{
		public string Html
		{
			get { return _html; }
			set
			{
				_html = value;
				Browser.Html = value;
			}
		}
		private string _html;

		public WebBrowserPage()
		{
			InitializeComponent();
			initToolbar();
		}

		public WebBrowserPage(string url)
		{
			InitializeComponent();
			initToolbar();
			Browser.Link = url;
		}

		/// <summary>
		/// Инициализировать панель управления
		/// </summary>
		private void initToolbar()
		{
			var goBack = new ToolbarItem { Text = "Назад", Icon = "back.png" };
			goBack.Clicked += (sender, args) => Browser.GoBack();
			ToolbarItems.Add(goBack);
			var goForward = new ToolbarItem { Text = "Вперед", Icon = "next.png" };
			ToolbarItems.Add(goForward);
		}
	}
}