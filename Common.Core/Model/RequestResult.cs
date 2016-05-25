using Newtonsoft.Json;

namespace Common.Core.Model
{
	/// <summary>
	/// Базовый класс для результата расчета
	/// </summary>
	public class RequestResult
	{
		/// <summary>
		/// Успешно ли выполенн расчет
		/// </summary>
		[JsonProperty("SUCCESS")]
		public bool Success { get; set; }

		/// <summary>
		/// Код ошибки.
		/// null если ошибки не было
		/// </summary>
		[JsonProperty("ERRORMESSAGE")]
		public string ErrorMessage { get; set; }
	}
}