namespace Common.Core
{
	/// <summary>
	/// Класс соответсвующий результату выполнения авторизации
	/// </summary>
	public class LoginInfo
	{
		/// <summary>
		/// Успешно ли выполнен расчет
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// Имя пользователя
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Идентификатор тикета
		/// </summary>
		public string Ticket { get; set; }

		/// <summary>
		/// Причина неудачного входа
		/// </summary>
		public string FailReason { get; set; }
	}
}