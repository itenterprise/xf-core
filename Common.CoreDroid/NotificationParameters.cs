using System;

namespace Common.CoreDroid
{
	/// <summary>
	///  ласс параметров отображени€ уведомлени€
	/// </summary>
	public class NotificationParameters
	{
		/// <summary>
		/// «аголовок уведомлени€
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// ѕризнак отображени€ уведомлени€
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// ѕризнак вибрировани€ при получении уведомлени€
		/// </summary>
		public bool Vibrate { get; set; }

		/// <summary>
		/// ѕризнак наличи€ светового сигнала при получении уведомлени€
		/// </summary>
		public bool MakeLight { get; set; }

		/// <summary>
		/// “ип активности, котора€ должна обработать уведомление
		/// </summary>
		public Type ActivityType { get; set; }

		/// <summary>
		/// »зображение уведомлени€
		/// </summary>
		public int Icon { get; set; }

		public NotificationParameters()
		{
			Title = string.Empty;
			Visible = true;
			Vibrate = true;
			ActivityType = typeof(MainActivityBase);
		}
	}
}