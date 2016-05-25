using System.Threading.Tasks;
using Common.Core.Helpers;
using Common.Core.Tools;
using Common.Core.WebService;
using Xamarin.Forms;

namespace Common.Core.ViewModel
{
	public class AuthenticationViewModel : UrlSettingsViewModel
	{
		public Command Authenticate
		{
			get { return _authenticate ?? (_authenticate = new Command(() => AttemptToAuthenticate(), () => IsNotBusy)); }
		}
		private Command _authenticate;

		public override string Url
		{
			get { return base.Url; }
			set
			{
				base.Url = value;
				trySetAccount(value);
			}
		}

		/// <summary>
		/// Иконка
		/// </summary>
		public string UserLogin
		{
			get { return _userLogin; }
			set { SetPropertyChanged(ref _userLogin, value); }
		}
		private string _userLogin = string.Empty;

		/// <summary>
		/// Иконка
		/// </summary>
		public string Password
		{
			get { return _password; }
			set { SetPropertyChanged(ref _password, value); }
		}
		private string _password = string.Empty;

		public string AuthenticationStatus
		{
			get { return _authenticationStatus; }
			set { SetPropertyChanged(ref _authenticationStatus, value); }
		}
		private string _authenticationStatus;

		public AuthenticationViewModel()
		{
			Title = Properties.Resources.Enter;
		}

		/// <summary>
		/// Попробовать залогиниться используя данные для входа из хранилища или из текущей сессии
		/// </summary>
		/// <returns></returns>
		public async Task<LoginInfo> TryLoginFromCurrentSessionOrSecureStorage()
		{
			AuthenticationStatus = Properties.Resources.SigningIn;
			LoginInfo result;
			using (new Busy(this))
			{
				result = await LoginHelper.TryLoginFromCurrentSessionOrSecureStorage();
			}
			AuthenticationStatus = "";
			return result;
		}

		public async Task<bool> AttemptToAuthenticate()
		{
			if (string.IsNullOrWhiteSpace(UserLogin) || string.IsNullOrWhiteSpace(Password))
			{
				Settings.Hud.DisplayError(Properties.Resources.EnterLoginAndPassword);
				return false;
			}
			if (!ApplicationBase.IsNetworkRechable)
			{
				Settings.Hud.DisplayError(Properties.Resources.NoInternet);
				return false;
			}
			AuthenticationStatus = Properties.Resources.SigningIn;
			using (new Busy(this))
			{
				await LoginHelper.Login(UserLogin, Password, true);
			}
			AuthenticationStatus = Properties.Resources.Done;
			var isAutenticated = Settings.IsAutenticated;
			if (isAutenticated)
			{
				ApplicationBase.Current.GotoMainPage();
			}
			return isAutenticated;
		}

		private void trySetAccount(string url)
		{
			if (string.IsNullOrEmpty(Url))
			{
				return;
			}
			var account = Settings.Utility.FindAccountForService(url);
			if (account == null)
			{
				return;
			}
			UserLogin = account.Login;
			Password = account.Password;
		}
	}
}