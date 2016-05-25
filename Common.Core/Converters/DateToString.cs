using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace Common.Core.Converters
{
	public class DateToString : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dt = ((DateTime)value);
			var timesince = DateTime.Now - dt;
			if (timesince.Days > 365)
			{
				var years = (timesince.Days / 365);
				if (timesince.Days % 365 != 0)
				{
					years += 1;
				}
				return GetYearsString(years);
			}
			if (timesince.Days > 30)
			{
				var months = (timesince.Days / 30);
				if (timesince.Days % 31 != 0)
				{
					months += 1;
				}
				return GetMonthsString(months);
			}
			if (timesince.Days > 0)
			{
				return GetDaysString(timesince.Days);
			}
			if (timesince.Hours > 0)
			{
				return GetHoursString(timesince.Hours);
			}
			if (timesince.Minutes > 0)
			{
				return GetMinutesString(timesince.Minutes);
			}
			if (timesince.Seconds > 5)
			{
				return GetSecondsString(timesince.Seconds);
			}
			return JustNowString;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		protected virtual string GetYearsString(int years)
		{
			return string.Format("около {0} {1} назад", years, years == 1 ? "года" : "лет");
		}

		protected virtual string GetMonthsString(int months)
		{
			return string.Format("около {0} {1} назад", months, months == 1 ? "месяца" : "месяцев");
		}

		protected virtual string GetDaysString(int days)
		{
			return string.Format("около {0} {1} назад", days, days == 1 ? "дня" : "дней");
		}

		protected virtual string GetHoursString(int hours)
		{
			return string.Format("около {0} {1} назад", hours, hours == 1 ? "часа" : "часов");
		}

		protected virtual string GetMinutesString(int minutes)
		{
			return string.Format("около {0} {1} назад", minutes, minutes == 1 ? "минуты" : "минут");
		}

		protected virtual string GetSecondsString(int seconds)
		{
			return string.Format("около {0} секунд назад", seconds);
		}

		protected virtual string JustNowString
		{
			get { return "Прямо сейчас"; }
		}
	}

	public class NullIntValueConverter : IValueConverter
	{
		public static NullIntValueConverter Instance = new NullIntValueConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? string.Empty : value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is string))
				return null;

			int i;
			if (int.TryParse((string)value, out i))
				return i;

			return null;
		}
	}

	public class InverseBoolConverter : IValueConverter
	{
		public static InverseBoolConverter Instance = new InverseBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}

	public class IsNotNullToBoolConverter : IValueConverter
	{
		public static IsNotNullToBoolConverter Instance = new IsNotNullToBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}
	}

	public class IsNullToBoolConverter : IValueConverter
	{
		public static IsNullToBoolConverter Instance = new IsNullToBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null;
		}
	}

	public class IsNotNullOrEmptyToBoolConverter : IValueConverter
	{
		public static IsNotNullToBoolConverter Instance = new IsNotNullToBoolConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var str = value as string;
			return !string.IsNullOrWhiteSpace(str);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}
	}

	public class IsEmptyConverter : IValueConverter
	{
		public static IsEmptyConverter Instance = new IsEmptyConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var list = value as IList;

			if (list == null)
				return false;

			return list.Count > 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var list = value as IList;

			if (list == null)
				return false;

			return list.Count > 0;
		}
	}

	public class TextPlaceholderConverter : IValueConverter
	{
		public static TextPlaceholderConverter Instance = new TextPlaceholderConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
				return "PlaceHolder text";
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
				return "PlaceHolder text";
			return value.ToString();
		}
	}
	public class TextPlaceholderColorConverter : IValueConverter
	{
		public static TextPlaceholderColorConverter Instance = new TextPlaceholderColorConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
				return Color.Gray;
			return Color.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
				return Color.Gray;
			return Color.Black;
		}
	}
}