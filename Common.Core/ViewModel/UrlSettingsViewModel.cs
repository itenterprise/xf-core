using Common.Core.Helpers;

namespace Common.Core.ViewModel
{
	public class UrlSettingsViewModel : BaseViewModel
	{
		/// <summary>
		/// URL веб-сервиса
		/// </summary>
		public virtual string Url
		{
			get { return _url; }
			set
			{
				SetPropertyChanged(ref _url, value);
				Settings.Url = value;
			}
		}
		private string _url;

		public UrlSettingsViewModel()
		{
			Title = Properties.Resources.Properties;
			Url = Settings.Url;
		}
	}
}