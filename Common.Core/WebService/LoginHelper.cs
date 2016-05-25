using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.Properties;
using Common.Core.Tools;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Common.Core.WebService
{
	/// <summary>
	/// Контролирует вход в систему
	/// </summary>
	public class LoginHelper
	{
		private static string _userLogin;
		private static string _userPassword;

		/// <summary>
		/// Попытаться войти в систему
		/// </summary>
		/// <param name="user">Логин</param>
		/// <param name="password">Пароль</param>
		/// <param name="remember">Признак сохранения учетных данных в хранилище</param>
		/// <returns></returns>
		public static async Task<LoginInfo> Login(string user, string password, bool remember)
		{
			if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
			{
				return null;
			}
			// попытаться войти
			var loginInfo = await login(user, password);
			if (loginInfo == null)
			{
				return null;
			}
			if (!loginInfo.Success)
			{
				return loginInfo;
			}
			_userLogin = Settings.UserLogin = user;
			_userPassword = password;
			Settings.UserName = loginInfo.UserName;
			Settings.Ticket = loginInfo.Ticket;
			if (remember)
			{
				Settings.Utility.Save(new Account {
					Login = user.ToUpper(),
					Password = password,
					ServerUrl = Settings.Url
				});
			}
			MessagingCenter.Send(ApplicationBase.Current, Messages.AuthenticationComplete);
			return loginInfo;
		}

		/// <summary>
		/// Попытаться войти, используя данные с хранилища
		/// </summary>
		/// <returns></returns>
		public static async Task<LoginInfo> TryLoginFromSecureStorage()
		{
			var credentials = GetLoginCreadentialsFromStorage();
			if (credentials != null)
			{
				return await Login(credentials.Login, credentials.Password, false);
			}
			return null;
		}

		/// <summary>
		/// Получить данные для входа из менеджера учетных записей
		/// </summary>
		/// <returns></returns>
		public static PasswordCredential GetLoginCreadentialsFromStorage()
		{
			var account = Settings.Utility.FindAccountForService(Settings.Url);
			if (account == null || string.IsNullOrEmpty(account.Login) || string.IsNullOrEmpty(account.Password))
			{
				return null;
			}
			return new PasswordCredential {
				Login = account.Login,
				Password = account.Password
			};
		}

		/// <summary>
		/// Выход
		/// </summary>
		public static void Logout()
		{
			removeStoredCredentials();
		}

		/// <summary>
		/// Попробовать залогиниться используя данные для входа из хранилища или из текущей сессии
		/// </summary>
		/// <returns></returns>
		public static async Task<LoginInfo> TryLoginFromCurrentSessionOrSecureStorage()
		{
			if (!string.IsNullOrEmpty(_userLogin) && !string.IsNullOrEmpty(_userPassword))
			{
				return await Login(_userLogin, _userPassword, true);
			}
			return await TryLoginFromSecureStorage();
		}

		/// <summary>
		/// Вход
		/// </summary>
		/// <param name="user">Логин</param>
		/// <param name="pass">Пароль</param>
		/// <returns>Имя пользователя, если успешно выполнен вход. Иначе - пусто</returns>
		private static async Task<LoginInfo> login(string user, string pass)
		{
			var json = await loginWithReconnect(user, pass);
			var loginInfo = JsonConvert.DeserializeObject<LoginInfo>(json);
			if (loginInfo != null)
			{
				if (!loginInfo.Success)
				{
					if (!string.IsNullOrEmpty(loginInfo.FailReason))
					{
						loginInfo.FailReason.ToToast(ToastEvent.Error);
					}
					else
					{
						Resources.IncorrectLoginOrPassword.ToToast(ToastEvent.Error);
					}
				}
			}
			else
			{
				Resources.CannotLogin.ToToast(ToastEvent.Error);
			}
			return loginInfo;
		}

		/// <summary>
		/// Удалить сохраненные данные для входа
		/// </summary>
		private static void removeStoredCredentials()
		{
			MessagingCenter.Send(ApplicationBase.Current, Messages.UserLogout);
			Settings.Utility.Delete(Settings.UserLogin, Settings.Url);
			Settings.UserLogin = Settings.UserName = Settings.Ticket = string.Empty;
			_userLogin = _userPassword = null;
		}


		/// <summary>
		/// Выполнить вход.
		/// Если произошли ошибки подключения - повторить попытку до 3-х раз
		/// </summary>
		/// <param name="user">Логин</param>
		/// <param name="pass">Пароль</param>
		/// <param name="trys">Кол-во попыток</param>
		/// <returns>json-результат выполнения веб-метода</returns>
		private static async Task<string> loginWithReconnect(string user, string pass, int trys = 0)
		{
			while (trys < 3)
			{
				var success = true;
				string responseStr = null;
				try
				{
					var executeExParams = new
					{
						login = user,
						password = pass
					};
					var urlParts = Settings.Url.Split('?');
					var url = new Uri(urlParts[0] + "/LoginEx?" + (urlParts.Length > 1 ? urlParts[1] : string.Empty) + "&pureJson=");
					var httpClient = new HttpClient();
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var executeExJson = JsonConvert.SerializeObject(executeExParams);
					var req = new HttpRequestMessage(HttpMethod.Post, url)
					{
						Headers = { AcceptLanguage = { new StringWithQualityHeaderValue(Settings.Culture)}},
						Content = new StringContent(executeExJson, Encoding.UTF8, "application/json")
					};
					var response = await httpClient.SendAsync(req);
					responseStr = await response.Content.ReadAsStringAsync();
				}
				catch (Exception e)
				{
					success = false;
					Resources.CannotConnectToServer.ToToast();
					ApplicationBase.TrackException(e);
				}
				if (success && string.IsNullOrEmpty(responseStr))
				{
					success = false;
				}
				if (success)
				{
					return responseStr;
				}
				trys = trys + 1;
			}
			return string.Empty;
		}
	}

	/// <summary>
	/// Параметры пользователя
	/// </summary>
	public class PasswordCredential
	{
		/// <summary>
		/// Логин
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Пароль
		/// </summary>
		public string Password { get; set; }
	}
}