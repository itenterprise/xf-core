using System;

namespace Common.CoreDroid
{
	/// <summary>
	/// ����� ���������� ����������� �����������
	/// </summary>
	public class NotificationParameters
	{
		/// <summary>
		/// ��������� �����������
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// ������� ����������� �����������
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// ������� ������������ ��� ��������� �����������
		/// </summary>
		public bool Vibrate { get; set; }

		/// <summary>
		/// ������� ������� ��������� ������� ��� ��������� �����������
		/// </summary>
		public bool MakeLight { get; set; }

		/// <summary>
		/// ��� ����������, ������� ������ ���������� �����������
		/// </summary>
		public Type ActivityType { get; set; }

		/// <summary>
		/// ����������� �����������
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