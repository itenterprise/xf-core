using System;
using System.Linq;
using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Common.Core.Tools
{
	/// <summary>
	/// Расширения
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Отобразить строку как всплывающее сообщение
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <param name="type">Тип сообщения</param>
		/// <param name="title">Заголовок сообщения</param>
		public static void ToToast(this string message, string title, ToastEvent type = ToastEvent.Info)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			if (string.IsNullOrEmpty(title))
			{
				message = tryCutTitle(message, out title);
			}
			UserDialogs.Instance.Toast(getToastConfig(type, title, message));
		}

		/// <summary>
		/// Отобразить строку как всплывающее сообщение
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <param name="type">Тип сообщения</param>
		public static void ToToast(this string message, ToastEvent type = ToastEvent.Info)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			string title;
			message = tryCutTitle(message, out title);
			UserDialogs.Instance.Toast(getToastConfig(type, title, message));
		}

		/// <summary>
		/// Отобразить строку как всплывающее сообщение
		/// </summary>
		/// <param name="message">Сообщение</param>
		/// <param name="title">Заголовок сообщения</param>
		/// <param name="maxLength">Максимальная длина</param>
		public static void ToToast(this string message, string title, int maxLength)
		{
			if (string.IsNullOrWhiteSpace(message))
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(title))
			{
				message = tryCutTitle(message, out title);
			}
			message = message.Length > maxLength
				? string.Format("{0}...", Text.Left(message, maxLength))
				: message;
			UserDialogs.Instance.Toast(getToastConfig(ToastEvent.Info, title, message));
		}

		private static ToastConfig getToastConfig(ToastEvent toastEvent, string title, string message)
		{
			if (string.IsNullOrWhiteSpace(title))
			{
				title = message;
				message = null;
			}
			return new ToastConfig(toastEvent, title, message)
			{
				BackgroundColor = System.Drawing.Color.FromArgb(44, 48, 54),
				TextColor = System.Drawing.Color.White,
				Duration = TimeSpan.FromSeconds(4)
			};
		}
		
		private static string tryCutTitle(string message, out string title)
		{
			title = string.Empty;
			var probableTitles = Text.Split(message, ':');
			if (probableTitles.Any() && probableTitles[0].Length < 30)
			{
				title = probableTitles[0];
				message = Text.Replace(message, string.Concat(probableTitles[0], ":"), "").Trim();
			}
			return message;
		}

		/// <summary>
		/// Форматировать строку
		/// </summary>
		/// <param name="s">Строка</param>
		/// <param name="args">Аргументы</param>
		public static string Fmt(this string s, params object[] args)
		{
			return string.Format(s, args);
		}

		private static readonly DateTime _unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0);

		/// <summary>
		/// Converts a <see cref="DateTime"/> object into a unix timestamp number.
		/// </summary>
		/// <param name="date">The date to convert.</param>
		/// <returns>A long for the number of seconds since 1st January 1970, as per unix specification.</returns>
		public static long ToUnixTimestamp(this DateTime date)
		{
			var ts = date - _unixStartDate;
			return (long)ts.TotalSeconds;
		}

		/// <summary>
		/// Converts a string, representing a unix timestamp number into a <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="timestamp">The timestamp, as a string.</param>
		/// <returns>The <see cref="DateTime"/> object the time represents.</returns>
		public static DateTime UnixTimestampToDate(this string timestamp)
		{
			if (timestamp == null || timestamp.Length == 0)
			{
				return DateTime.MinValue;
			}
			return UnixTimestampToDate(long.Parse(timestamp));
		}

		/// <summary>
		/// Converts a <see cref="long"/>, representing a unix timestamp number into a <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="timestamp">The unix timestamp.</param>
		/// <returns>The <see cref="DateTime"/> object the time represents.</returns>
		public static DateTime UnixTimestampToDate(this long timestamp)
		{
			return _unixStartDate.AddSeconds(timestamp);
		}
	}

	[ContentProperty("Source")]
	public class ImageResourceExtension : IMarkupExtension
	{
		public string Source { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Source == null)
				return null;

			// Do your translation lookup here, using whatever method you require
			var imageSource = ImageSource.FromResource(Source);

			return imageSource;
		}
	}
}