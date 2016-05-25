using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Core;

/// <summary>
/// Класс для работы с вложениями
/// </summary>
public static class AttachmentRepository
{
	/// <summary>
	/// Получить список вложений
	/// </summary>
	/// <param name="table">Код таблицы</param>
	/// <param name="keyValue">Значение ключа строки</param>
	/// <returns></returns>
	public static Task<List<Attachment>> GetList(string table, string keyValue)
	{
		return WebServiceHelper.Execute<List<Attachment>>("GETALLATTACHMENTS", new {
			Table = table,
			KeyValue = keyValue
		});
	}
}