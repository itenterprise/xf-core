using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common.Core.Tools
{
	/// <summary>
	/// Утилиты для работы с URL
	/// </summary>
	public sealed class HttpUtility
	{
		/// <summary>
		/// Декодировать URL
		/// </summary>
		public static string UrlDecode(string url)
		{
			return WebUtility.UrlDecode(url);
		}

		/// <summary>
		/// Кодировать URL
		/// </summary>
		public static string UrlEncode(string url)
		{
			return WebUtility.UrlEncode(url);
		}

		public static Dictionary<string, string> ParseQueryString(string query)
		{
			return ParseQueryString(query, Encoding.UTF8);
		}

		/// <summary>
		/// Получить параметры запроса
		/// </summary>
		/// <param name="query">Запрос</param>
		/// <param name="encoding">Кодировка</param>
		/// <returns>Параметры запроса по ключам</returns>
		public static Dictionary<string, string> ParseQueryString(string query, Encoding encoding)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			if (encoding == null)
				throw new ArgumentNullException("encoding");
			if (query.Length == 0 || (query.Length == 1 && query[0] == '?'))
				return new HttpQsCollection();
			if (query[0] == '?')
				query = query.Substring(1);

			var result = new HttpQsCollection();
			ParseQueryString(query, encoding, result);
			return result;
		}

		internal static void ParseQueryString(string query, Encoding encoding, Dictionary<string, string> result)
		{
			if (query.Length == 0)
			{
				return;
			}

			var decoded = WebUtility.HtmlDecode(query);
			var decodedLength = decoded.Length;
			var namePos = 0;
			var first = true;
			while (namePos <= decodedLength)
			{
				int valuePos = -1, valueEnd = -1;
				for (int q = namePos; q < decodedLength; q++)
				{
					if (valuePos == -1 && decoded[q] == '=')
					{
						valuePos = q + 1;
					}
					else if (decoded[q] == '&')
					{
						valueEnd = q;
						break;
					}
				}

				if (first)
				{
					first = false;
					if (decoded[namePos] == '?')
						namePos++;
				}

				string name;
				if (valuePos == -1)
				{
					name = null;
					valuePos = namePos;
				}
				else
				{
					name = UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
				}
				if (valueEnd < 0)
				{
					namePos = -1;
					valueEnd = decoded.Length;
				}
				else
				{
					namePos = valueEnd + 1;
				}
				var value = WebUtility.UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));

				result.Add(name, value);
				if (namePos == -1)
					break;
			}
		}

		sealed class HttpQsCollection : Dictionary<string, string>
		{
			public override string ToString()
			{
				var count = Count;
				if (count == 0)
				{
					return "";
				}
				var sb = new StringBuilder();
				foreach (var key in Keys)
				{
					sb.AppendFormat("{0}={1}&", key, this[key]);
				}
				if (sb.Length > 0)
					sb.Length--;
				return sb.ToString();
			}
		}
	}
}