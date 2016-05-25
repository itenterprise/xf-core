namespace Common.Core.Converters
{
	public class DateToShortString : DateToString
	{
		protected override string GetYearsString(int years)
		{
			return string.Format("{0}{1}", years, "г");
		}

		protected override string GetMonthsString(int months)
		{
			return string.Format("{0}{1}", months, "мес");
		}

		protected override string GetDaysString(int days)
		{
			return string.Format("{0}{1}", days, "д");
		}

		protected override string GetHoursString(int hours)
		{
			return string.Format("{0}{1}", hours, "ч");
		}

		protected override string GetMinutesString(int minutes)
		{
			return string.Format("{0}{1}", minutes, "м");
		}

		protected override string GetSecondsString(int seconds)
		{
			return string.Format("{0}с", seconds);
		}
	}
}