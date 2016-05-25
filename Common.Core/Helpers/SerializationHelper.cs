using Newtonsoft.Json;

namespace Common.Core.Helpers
{
	/// <summary>
	/// Класс для сериализации
	/// </summary>
	public static class SerializationHelper
	{
		/// <summary>
		/// Сериализация обьекта в строку
		/// </summary>
		/// <param name="object">Обьект</param>
		public static string Serialize(object @object)
		{
			if (@object == null)
			{
				return null;
			}
			return JsonConvert.SerializeObject(@object);
		}

		/// <summary>
		/// Десериализация строки в обьект указанного типа
		/// </summary>
		/// <param name="serializedObject">Сериализованная строка</param>
		/// <typeparam name="T">Требуемый тип десериализованного обьекта</typeparam>
		public static T Deserialize<T>(string serializedObject)
		{
			if (string.IsNullOrEmpty(serializedObject))
			{
				return default(T);
			}
			return JsonConvert.DeserializeObject<T>(serializedObject);
		}
	}
}