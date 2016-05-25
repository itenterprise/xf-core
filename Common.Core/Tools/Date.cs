using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Core.Tools
{
	/// <summary>
	/// Работа с датами
	/// </summary>
	public static class Date
	{
		/// <summary>
		/// Определение первой даты месяца
		/// </summary>
		/// <param name="year">Год</param>
		/// <param name="month">Месяц</param>
		/// <returns>Первая дата месяца</returns>
		public static DateTime FirstDateOfMonth(int year, int month)
		{
			if (!validYearMonth(year, month))
			{
				return DateTime.MinValue;
			}
			return new DateTime(year, month, 1);
		}

		/// <summary>
		/// Определение первой даты месяца
		/// </summary>
		/// <param name="yearMonth">Дата в формате YYYYMM</param>
		/// <returns>Первая дата месяца</returns>
		public static DateTime FirstDateOfMonth(string yearMonth)
		{
			int year;
			int month;
			YearAndMonthFromString(yearMonth, out year, out month);
			return String.IsNullOrEmpty(yearMonth)
				? DateTime.MinValue
				: FirstDateOfMonth(year, month);
		}

		/// <summary>
		/// Определение первой даты месяца
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Первая дата месяца</returns>
		public static DateTime FirstDateOfMonth(DateTime date)
		{
			return FirstDateOfMonth(date.Year, date.Month);
		}

		/// <summary>
		/// Определение последней даты месяца
		/// </summary>
		/// <param name="year">Год</param>
		/// <param name="month">Месяц</param>
		/// <returns>Последняя дата месяца</returns>
		public static DateTime LastDateOfMonth(int year, int month)
		{
			if (!validYearMonth(year, month))
			{
				return DateTime.MinValue;
			}
			if (year == DateTime.MaxValue.Year && month == DateTime.MaxValue.Month)
			{
				return DateTime.MaxValue.Date;
			}
			DateTime date = new DateTime(year, month, 1);
			// Прибавляем месяц и отнимаем день
			return date.AddMonths(1).AddDays(-1);
		}

		/// <summary>
		/// Определение последней даты месяца
		/// </summary>
		/// <param name="yearMonth">Дата в формате YYYYMM</param>
		/// <returns>Последняя дата месяца</returns>
		public static DateTime LastDateOfMonth(string yearMonth)
		{
			int year;
			int month;
			YearAndMonthFromString(yearMonth, out year, out month);
			return String.IsNullOrEmpty(yearMonth)
				? DateTime.MinValue
				: LastDateOfMonth(year, month);
		}

		/// <summary>
		/// Определение последней даты месяца
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Последняя дата месяца</returns>
		public static DateTime LastDateOfMonth(DateTime date)
		{
			return LastDateOfMonth(date.Year, date.Month);
		}
		
		/// <summary>
		/// Получить строку вида ГГГГММ по году и месяцу
		/// </summary>
		/// <param name="year">Год</param>
		/// <param name="month">Месяц</param>
		/// <returns>Строка вида ГГГГММ. Возвращает пустую строку, если месяц или год выходят за пределы доступных диапазонов</returns>
		public static string GetYearMonthString(int year, int month)
		{
			return validYearMonth(year, month) ? String.Format("{0}{1}", year.ToString("0000"), month.ToString("00")) : string.Empty;
		}

		private static bool validYearMonth(int year, int month)
		{
			return !(year < 1 || year > 9999 || month < 1 || month > 12);
		}

		/// <summary>
		/// Преобразовать дату в строку вида ГГГГММ
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Дата в формате ГГГГММ. Если дата - <see cref="DateTime.MinValue"/>, то возвращается пустая строка</returns>
		public static string GetYearMonthString(DateTime date)
		{
			if (date == DateTime.MinValue)
			{
				return String.Empty;
			}
			return GetYearMonthString(date.Year, date.Month);
		}

		/// <summary>
		/// Получить год и месяц из строки ГГГГММ
		/// </summary>
		/// <param name="yearMonth">Строка ГГГГММ</param>
		/// <param name="year">Выходной параметр, возвращает год</param>
		/// <param name="month">Выходной параметр, возвращает месяц</param>
		/// <remarks>Метод возвращает нулевые год и месяц, если передано строку неправильного формата</remarks>
		public static void YearAndMonthFromString(string yearMonth, out int year, out int month)
		{
			var trimmedYearMonth = yearMonth != null ? yearMonth.Trim() : null;
			if (String.IsNullOrEmpty(trimmedYearMonth) || trimmedYearMonth.Length != 6 || !Text.IsNumber(trimmedYearMonth, false))
			{
				year = month = 0;
			}
			else
			{
				year = Convert.ToInt32(trimmedYearMonth.Substring(0, 4));
				month = Convert.ToInt32(trimmedYearMonth.Substring(4, 2));
				CheckYearMonth(ref year, ref month);
			}
		}

		/// <summary>
		/// Проверить значения года и месяца и обнулить их, если хотя б один из них задан не верно
		/// </summary>
		/// <param name="year">Год</param>
		/// <param name="month">Месяц</param>
		internal static void CheckYearMonth(ref int year, ref int month)
		{
			month = month < 1 || month > 12 ? 0 : month;
			year = year < 0 || year > 9999 ? 0 : year;
			// Если какой-то из параметров задан неверно, то считается, что оба заданы неверно
			if (month == 0 || year == 0)
			{
				year = month = 0;
			}
		}

		/// <summary>
		/// Получить номер дня недели. ПН => 1 ... ВС => 7
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Номер дня недели</returns>
		/// <remarks>Метод возвращает 0, если в качестве аргумента передан <c>DateTime.MinValue</c></remarks>
		public static int DayOfWeek(DateTime date)
		{
			int day = 0;
			if (date != DateTime.MinValue)
			{
				day = (int)date.DayOfWeek;
				// Если текущий день воскресенье, возвращаем 7
				if (day == 0)
				{
					day = 7;
				}
			}
			return day;
		}

		/// <summary>
		/// Получить номер недели года
		/// </summary>
		/// <param name="date">Дата</param>
		/// <remarks>Если часть недели содержится в одном году, а часть в другом, то для первого эта неделя будет последней, а для второго - первой</remarks>
		/// <remarks>Метод возвращает 0, если в качестве аргумента передан <c>DateTime.MinValue</c></remarks>
		/// <returns>Номер недели года</returns>
		public static int WeekOfYear(DateTime date)
		{
			int week = 0;
			if (date != DateTime.MinValue)
			{
				var firstDayOfYear = new DateTime(date.Year, 1, 1);
				//столько дней первой недели данного года осталось в прошлом году
				var daysInPrevYear = (DayOfWeek(firstDayOfYear) - 1);
				week = (date.DayOfYear + daysInPrevYear - 1) / 7 + 1;
			}
			return week;
		}

		/// <summary>
		/// Получить номер недели месяца
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Номер недели месяца</returns>
		/// <remarks>Метод возвращает 0, если в качестве аргумента передан <c>DateTime.MinValue</c></remarks>
		public static int WeekOfMonth(DateTime date)
		{
			int week = 0;
			if (date != DateTime.MinValue)
			{
				var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
				week = WeekOfYear(date) - WeekOfYear(firstDayOfMonth) + 1;
			}
			return week;
		}

		/// <summary>
		/// Получить номер квартала года
		/// </summary>
		/// <param name="date">Дата</param>
		/// <returns>Номер квартала года</returns>
		/// <remarks>Метод возвращает 0, если в качестве аргумента передан <c>DateTime.MinValue</c></remarks>
		public static int QuarterOfYear(DateTime date)
		{
			int quarter = 0;
			if (date != DateTime.MinValue)
			{
				quarter = (date.Month - 1) / 3 + 1;
			}
			return quarter;
		}

		/// <summary>
		/// Получить максимальную дату-время среди переданных
		/// </summary>
		/// <param name="dates">Даты-времена, среди которых необходимо определить максимальную</param>
		/// <returns>Максимальную дату-время</returns>
		/// <remarks>Метод возвращает <c>DateTime.MinValue</c>, если ничего не передано</remarks>
		public static DateTime Max(params DateTime[] dates)
		{
			return evaluate(dates, d => d.Max());
		}

		/// <summary>
		/// Получить минимальную дату-время среди переданных
		/// </summary>
		/// <param name="dates">Даты-времена, среди которых необходимо определить минимальную</param>
		/// <returns>Минимальную дату-время</returns>
		/// <remarks>Метод возвращает <c>DateTime.MinValue</c>, если ничего не передано</remarks>
		public static DateTime Min(params DateTime[] dates)
		{
			return evaluate(dates, d => d.Min());
		}

		private static DateTime evaluate(IEnumerable<DateTime> dates, Func<IEnumerable<DateTime>, DateTime> function)
		{
			if (dates == null || !dates.Any())
			{
				return DateTime.MinValue;
			}
			return function(dates);
		}
	}
}