using System;
using System.Collections.Generic;
using System.Globalization;
using Common.Core.Interfaces;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace Common.Core.Helpers
{
	/// <summary>
	/// Класс глобальных настроек
	/// </summary>
	public class Settings
	{
		public static bool IsAutenticated
		{
			get { return !string.IsNullOrEmpty(Ticket) && !string.IsNullOrEmpty(UserLogin); }
		}

		/// <summary>
		/// Логин пользователя
		/// </summary>
		public static string UserLogin
		{
			get { return AppSettings.GetValueOrDefault<string>(_userLoginKey); }
			set { AppSettings.AddOrUpdateValue(_userLoginKey, value); }
		}

		private static string _userLoginKey = "USERLOGINKEY";

		/// <summary>
		/// Имя пользователя
		/// </summary>
		public static string UserName
		{
			get { return AppSettings.GetValueOrDefault<string>(_userNameKey); }
			set { AppSettings.AddOrUpdateValue(_userNameKey, value); }
		}

		private static string _userNameKey = "USERNAMEKEY";

		/// <summary>
		/// Идентификатор тикета
		/// </summary>
		public static string Ticket { get; set; }

		/// <summary>
		/// Адрес веб-сервера
		/// </summary>
		public static string Url
		{
			get
			{
				var currentApp = ApplicationBase.Current;
				return AppSettings.GetValueOrDefault(_urlKey, currentApp == null ? null : currentApp.DefaultUrl);
			}
			set { AppSettings.AddOrUpdateValue(_urlKey, value); }
		}

		private const string _urlKey = "URLKEY";

		/// <summary>
		/// Текущий язык
		/// </summary>
		public static string Culture
		{
			get
			{
				return AppSettings.GetValueOrDefault(_culture, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_culture, value);
			}
		}
		private const string _culture = "CULTURE";

		/// <summary>
		/// Идентификатор последнего уведомления
		/// </summary>
		public static int LastNotificationId
		{
			get
			{
				return AppSettings.GetValueOrDefault(_lastNotificationId, 1);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_lastNotificationId, value);
			}
		}
		private const string _lastNotificationId = "LASTNID";

		/// <summary>
		/// Push-уведомления
		/// </summary>
		public static bool SignalAboutNotifications
		{
			get
			{
				return AppSettings.GetValueOrDefault(_signalAboutNotificationsKey, true);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_signalAboutNotificationsKey, value);
				if (value)
				{
					ApplicationBase.Current.SubscribePushNotifications(Sound);
				}
				else
				{
					ApplicationBase.Current.UnsubscribePushNotifications();
				}
			}
		}
		private const string _signalAboutNotificationsKey = "SIGNALABOUTNOTIFICATIONS";

		/// <summary>
		/// Звуковой сигнал о получении нового уведомления
		/// </summary>
		public static bool Sound
		{
			get
			{
				return AppSettings.GetValueOrDefault(_soundKey, true);
			}
			set
			{
				AppSettings.AddOrUpdateValue(_soundKey, value);
				ApplicationBase.Current.SubscribePushNotifications(value);
			}
		}
		private const string _soundKey = "NOTIFICATIONSSOUND";

		public static IHUDProvider Hud
		{
			get
			{
				return _hud ?? (_hud = DependencyService.Get<IHUDProvider>());
			}
		}
		public static IHUDProvider _hud;

		public static IUtility Utility
		{
			get
			{
				return _utility ?? (_utility = DependencyService.Get<IUtility>());
			}
		}
		public static IUtility _utility;

		/// <summary>
		/// Идентификатор получателя push-сообщний от IT-Enterprise
		/// </summary>
		public static string NotificationRegistrationId
		{
			get { return AppSettings.GetValueOrDefault<string>(_notificationRegistrationIdKey); }
			set { AppSettings.AddOrUpdateValue(_notificationRegistrationIdKey, value); }
		}

		private static readonly string _notificationRegistrationIdKey = "notificationRegistrationId";

		/// <summary>
		/// Перечень показанных экранов в режиме обучения
		/// </summary>
		public static HashSet<string> ShowedTips
		{
			get
			{
				var showedTipsSerialized = AppSettings.GetValueOrDefault(_showedTipsKey, string.Empty);
				var showedTips = SerializationHelper.Deserialize<HashSet<string>>(showedTipsSerialized);
				if (showedTips != null)
				{
					return showedTips;
				}
				showedTips = new HashSet<string>();
				AppSettings.AddOrUpdateValue(_showedTipsKey, SerializationHelper.Serialize(showedTips));
				return showedTips;
			}
			set
			{
				var serialized = SerializationHelper.Serialize(value);
				AppSettings.AddOrUpdateValue(_showedTipsKey, serialized);
			}
		}
		private static readonly string _showedTipsKey = "ShowedTips";

		/// <summary>
		/// Уникальный номер пользователя для Google-аналитики
		/// </summary>
		public static string GoogleAnalyticsUserId
		{
			get
			{
				var value = AppSettings.GetValueOrDefault(_uniqUserGuid, Guid.NewGuid().ToString());
				AppSettings.AddOrUpdateValue(_uniqUserGuid, value);
				return value;
			}
			set
			{
				AppSettings.AddOrUpdateValue(_uniqUserGuid, value);
			}
		}
		private static readonly string _uniqUserGuid = "UniqUserGuid";

		protected static ISettings AppSettings
		{
			get { return CrossSettings.Current; }
		}
	}
}