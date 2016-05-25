using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace Common.Core.Tools
{
	/// <summary>
	/// Работа с текстом
	/// </summary>
	public class Text
	{
		///<summary>
		/// Формат представления даты
		///</summary>
		[Flags]
		public enum DateTimeView
		{
			/// <summary>
			/// Дата и время. Пример: 31.12.2007 14:37
			/// </summary>
			DateTime = 0,
			/// <summary>
			/// Дата. Пример: 31.12.2007
			/// </summary>
			Date = 1,
			/// <summary>
			/// Время. Пример: 14:37
			/// </summary>
			Time = 2,
			/// <summary>
			/// Timestamp (yyyyMMddHHmmss). Пример: 20071231143721
			/// </summary>
			TimeStamp = 4,
			/// <summary>
			/// Отображать 2 знака в году. 
			/// Пример: 31.12.07. Или для даты с временем: 31.12.07 14:37
			/// </summary>
			Year2 = 8,
			/// <summary>
			/// Отображать время с секундами. 
			/// Пример: 14:37:21. Для даты с временем: 31.12.2007 14:37:21
			/// </summary>
			Seconds = 16,
			/// <summary>
			/// YyyyMmDd. Пример: 20071231
			/// </summary>
			YyyyMmDd = TimeStamp | Date
		}

		/// <summary>
		/// Конвертировать дату и время в строку, используя указанный формат представления даты
		/// </summary>
		/// <param name="value">Дата для конвертации</param>
		/// <param name="view">Формат представления даты</param>
		/// <returns>Строковое описание даты</returns>
		[Pure]
		public static string Convert(DateTime value, DateTimeView view)
		{
			if (value == DateTime.MinValue)
			{
				return string.Empty;
			}
			return value.ToString(GetDateTimeFormat(view), DateTimeFormat);
		}

		/// <summary>
		/// Формат с точкой-разделителем целой и дробной частей
		/// </summary>
		public static readonly NumberFormatInfo PointDecimalSeparator =
			new NumberFormatInfo { NumberDecimalSeparator = "." };

		internal static readonly DateTimeFormatInfo DateTimeFormat =
			new DateTimeFormatInfo
			{
				ShortDatePattern = "dd.MM.yy",
				LongDatePattern = "dd.MM.yyyy",
				ShortTimePattern = "HH:mm",
				LongTimePattern = "HH:mm:ss"
			};

		const string _timeStampFormat = "yyyyMMddHHmmss";
		/// <summary>
		/// Получить строку формата по указанным флагам представления даты
		/// </summary>
		/// <param name="view">Формат представления даты</param>
		/// <returns>Строка формата</returns>
		[Pure]
		public static string GetDateTimeFormat(DateTimeView view)
		{
			string format;

			if ((view & DateTimeView.TimeStamp) == DateTimeView.TimeStamp)
			{
				if ((view & DateTimeView.Date) == DateTimeView.Date)
				{
					//Если выбран режим TimeStamp + Date
					format = _timeStampFormat.Substring(0, 8);
				}
				else if ((view & DateTimeView.Time) == DateTimeView.Time && (view & DateTimeView.Seconds) == DateTimeView.Seconds)
				{
					format = String.Format("{0} {1}", _timeStampFormat.Substring(0, 8), DateTimeFormat.LongTimePattern);
				}
				else
				{
					//Если выбран режим TimeStamp
					format = _timeStampFormat;
				}
			}
			else if ((view & DateTimeView.Date) == DateTimeView.Date)
			{
				// Дата
				format = (view & DateTimeView.Year2) == DateTimeView.Year2
					? DateTimeFormat.ShortDatePattern
					: DateTimeFormat.LongDatePattern;
			}
			else if ((view & DateTimeView.Time) == DateTimeView.Time)
			{
				// Время
				format = (view & DateTimeView.Seconds) == DateTimeView.Seconds
					? DateTimeFormat.LongTimePattern
					: DateTimeFormat.ShortTimePattern;
			}
			else
			{
				// Дата и время
				format = String.Format("{0} {1}",
					(view & DateTimeView.Year2) == DateTimeView.Year2
						? DateTimeFormat.ShortDatePattern
						: DateTimeFormat.LongDatePattern,
					(view & DateTimeView.Seconds) == DateTimeView.Seconds
						? DateTimeFormat.LongTimePattern
						: DateTimeFormat.ShortTimePattern);
			}
			return format;
		}

		/// <summary>
		/// Конвертирует строку в base64
		/// </summary>
		/// <param name="str"></param>
		/// <param name="encoding">Кодировка строки. По умолчанию - UTF8</param>
		/// <returns></returns>
		[Pure]
		public static string ToBase64(string str, Encoding encoding = null)
		{
			var stringEncoding = encoding ?? Encoding.UTF8;
			var bytes = stringEncoding.GetBytes(str);
			return System.Convert.ToBase64String(bytes);
		}

		/// <summary>
		/// Строка является числом
		/// </summary>
		/// <param name="s">Проверяемая строка</param>
		/// <param name="numberHasDot">Может содержать точку-разделитель целой и дробной частей. По умолчанию - может содержать</param>
		/// <returns></returns>
		[Pure]
		public static bool IsNumber(string s, bool numberHasDot = true)
		{
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			bool hasDot = !numberHasDot;
			// Проверить, является ли число отрицательным
			bool isNegative = s[0] == '-';
			// Проверить, чтобы строка не состоялась только с минуса
			if (isNegative && s.Length == 1)
			{
				return false;
			}
			for (var i = isNegative ? 1 : 0; i < s.Length; i++)
			{
				var c = s[i];
				if (c == '.')
				{
					if (hasDot)
					{
						return false;
					}
					hasDot = true;
					continue;
				}
				if (!Char.IsDigit(c))
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// Символ является частью слова
		/// </summary>
		/// <param name="ch">Проверяемый символ</param>
		/// <returns></returns>
		[Pure]
		public static bool IsPartOfWord(char ch)
		{
			return ch == '_' || Char.IsLetterOrDigit(ch);
		}

		/// <summary>
		/// Строка может использоваться как идентификатор:
		/// Содержит буквы цифры и подчеркивание и начинается с буквы или "_"
		/// </summary>
		/// <param name="str">Строка</param>
		/// <param name="canBeQuotable">Идентификатор может быть заключен в двойные кавычки</param>
		/// <returns></returns>
		[Pure]
		public static bool IsValidIdentifier(string str, bool canBeQuotable = false)
		{
			if (String.IsNullOrEmpty(str))
			{
				return false;
			}
			var chars = str.ToCharArray();

			// определить индекс с которого начинать проверку
			int startIndex;
			int endIndex;
			if (canBeQuotable && chars[0] == '"' && chars[chars.Length - 1] == '"')
			{
				startIndex = 1;
				endIndex = chars.Length - 1;
			}
			else
			{
				startIndex = 0;
				endIndex = chars.Length;
			}

			if (!Char.IsLetter(chars[startIndex]) && chars[startIndex] != '_')
			{
				return false;
			}
			for (int i = startIndex; i < endIndex; i++)
			{
				if (!IsPartOfWord(chars[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Проверяет наличие идентификатора в строке. Функция регистронезависима
		/// </summary>
		/// <param name="str">Проверяемая строка</param>
		/// <param name="identifier">Идентификатор</param>
		/// <returns></returns>
		[Pure]
		public static bool ContainsIdentifier(string str, string identifier)
		{
			if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(identifier))
			{
				return false;
			}
			var index = 0;
			var success = false;
			do
			{
				index = str.IndexOf(identifier, index, StringComparison.OrdinalIgnoreCase);
				if (index != -1)
				{
					var endIndex = index + identifier.Length;
					success = (index == 0 || !IsPartOfWord(str[index - 1])) && (endIndex == str.Length || !IsPartOfWord(str[endIndex]));
					if (success)
					{
						break;
					}
					index = endIndex;
				}
			} while (index != -1);
			return success;
		}

		/// <summary>
		/// Подсчитать количество вхождений подстроки в строку
		/// </summary>
		/// <param name="search">"Что ищем" - подстрока, которая ищется</param>
		/// <param name="searched">"В чем ищем" - строка, в которой ищутся вхождения</param>
		/// <returns>Количество вхождений</returns>
		[Pure]
		public static int Occurs(string search, string searched)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(searched))
			{
				for (int i = 0; i <= searched.Length - search.Length; ++i)
				{
					if (String.CompareOrdinal(searched, i, search, 0, search.Length) == 0)
					{
						i += search.Length - 1;
						++result;
					}
				}
			}

			//#if DEBUG
			//            int oldEngineResult = (searched.Length - searched.Replace(search, String.Empty).Length) / search.Length;
			//            Debug.Assert(result == oldEngineResult);
			//#endif 
			return result;
		}

		/// <summary>
		/// Возвращает символьную строку, созданную заменой заданного числа символов в символьном выражении другим символьным выражением
		/// </summary>
		/// <param name="expression">Задает символьное выражение, в котором происходит замещение</param>
		/// <param name="startReplacement">Задает позицию выражения, начиная c которой происходит замещение (нумерация с 0).
		/// Если задана позиция меньше нуля, то считается равной нулю.
		/// Если символ с указанной позицией отсутствует в строке (строка короче), то строка дополняется пробелами до требуемой длины.</param>
		/// <param name="charactersReplaced">Задает число символов, подлежащих замещению. 
		/// Если задано отрицательное количество, то количество считается равным нулю.</param>
		/// <param name="replacement">Задает замещающее символьное выражение</param>
		/// <returns></returns>
		[Pure]
		public static string Stuff(string expression, int startReplacement, int charactersReplaced, string replacement)
		{
			startReplacement = Math.Max(startReplacement, 0);
			charactersReplaced = Math.Max(charactersReplaced, 0);
			var sb = new StringBuilder(expression);
			if (sb.Length < startReplacement + charactersReplaced)
			{
				sb.Append(' ', startReplacement + charactersReplaced - sb.Length);
			}
			sb.Remove(startReplacement, charactersReplaced);
			sb.Insert(startReplacement, replacement);
			return sb.ToString();
		}

		/// <summary>
		/// Повторить строку заданное количество раз
		/// </summary>
		/// <param name="text">Строка</param>
		/// <param name="count">Количество повторений</param>
		/// <returns>Результат</returns>
		[Pure]
		public static string Replicate(string text, int count)
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < count; ++i)
			{
				result.Append(text);
			}
			return result.ToString();
		}

		/// <summary>
		/// Получить заданное количество символов из выражения, начиная с самого левого символа
		/// </summary>
		/// <param name="text">Выражение</param>
		/// <param name="length">Количество символов. Если оно больше чем длина исходной строки, то функция вернет исходное выражение</param>
		/// <returns></returns>
		[Pure]
		public static string Left(string text, int length)
		{
			if (String.IsNullOrEmpty(text) || text.Length <= length)
			{
				return text ?? String.Empty;
			}
			return text.Substring(0, length);
		}

		/// <summary>
		/// Получить заданное количество символов из выражения, начиная с самого правого символа
		/// </summary>
		/// <param name="text">Выражение</param>
		/// <param name="length">Количество символов. Если оно больше чем длина исходной строки, то функция вернет исходное выражение</param>
		/// <returns></returns>
		[Pure]
		public static string Right(string text, int length)
		{
			if (String.IsNullOrEmpty(text))
			{
				return String.Empty;
			}
			int textLenght = text.Length;
			return textLenght <= length ? text : text.Substring(textLenght - length, length);
		}

		/// <summary>
		/// Заменить символы перехода на новую строку на "\r\n" (формат, принятый в Windows)
		/// </summary>
		public static string NewLinesToRn(string text)
		{
			if (String.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return text.Replace("\r\n", "\r").Replace("\n", "\r").Replace("\r", "\r\n");
		}

		/// <summary>
		/// Заменить символы перехода на новую строку на "\r"
		/// </summary>
		public static string NewLinesToR(string text)
		{
			if (String.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return text.Replace("\r\n", "\r").Replace('\n', '\r');
		}

		/// <summary>
		/// Получить первые N строк текста
		/// </summary>
		/// <param name="text"></param>
		/// <param name="linesCount"></param>
		/// <returns></returns>
		public static string FirstLines(string text, int linesCount)
		{
			if (String.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			string[] lines = SplitToLines(text);
			if (lines.Length <= linesCount)
			{
				return text;
			}
			var result = new StringBuilder();
			for (int i = 0; i < Math.Min(linesCount, lines.Length); ++i)
			{
				result.AppendLine(lines[i]);
			}
			result.Append("...");
			return result.ToString();
		}

		/// <summary>
		/// Разделить текст на строки.
		/// </summary>
		/// <param name="text">Входной текст.</param>
		/// <param name="keepEmptyLines">Сохранять пустые строки.</param>
		/// <returns>Массив строк.</returns>
		public static string[] SplitToLines(string text, bool keepEmptyLines = true)
		{
			if (String.IsNullOrEmpty(text))
			{
				if (text == null)
				{
					return new string[] { };
				}
				return keepEmptyLines ? new string[] { string.Empty } : new string[] { };
			}
			var lines = text.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
			if (!keepEmptyLines)
			{
				lines = Array.FindAll(lines, str => str.Trim() != string.Empty);
			}
			return lines;
		}

		/// <summary>
		/// Вернуть первую строку текста
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string FirstLine(string text)
		{
			if (String.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			int newLinePosition = text.IndexOfAny(new[] { '\n', '\r' });
			return newLinePosition != -1 ? text.Substring(0, newLinePosition) : text;
		}

		/// <summary>
		/// Возвращает строковый массив, содержащий подстроки текста, ограниченные разделителями.
		/// Если текст пустой или null, возвращает пустой массив
		/// </summary>
		/// <param name="text">Исходный текст</param>
		/// <param name="separator">Разделитель</param>
		/// <param name="keepEmptyLines">Признак сохранения пустых строк</param>
		/// <returns></returns>
		[Pure]
		public static string[] Split(string text, char separator = ',', bool keepEmptyLines = false)
		{
			return Split(text, new[] { separator }, keepEmptyLines);
		}

		/// <summary>
		/// Возвращает строковый массив, содержащий подстроки текста, ограниченные разделителями.
		/// Если текст пустой или null, возвращает пустой массив
		/// </summary>
		/// <param name="text">Исходный текст</param>
		/// <param name="separators">Список разделителей</param>
		/// <param name="keepEmptyLines">Признак сохранения пустых строк</param>
		/// <returns></returns>
		[Pure]
		public static string[] Split(string text, char[] separators, bool keepEmptyLines = false)
		{
			if (string.IsNullOrEmpty(text))
			{
				return new string[] { };
			}
			string[] stringArray = text.Split(separators, keepEmptyLines ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
			return stringArray;
		}

		/// <summary>Проверяет соответствие строки на количество открытых/закрытых скобок</summary>
		/// <param name="brackets">Набор скобок</param>
		/// <param name="line">Строка, которая проверяется</param>
		private static bool checkBrackets(string[,] brackets, string line)
		{
			for (int i = 0; i < 2; i++)
			{
				// Количество незакрытых скобок
				int number = brackets[i, 0] == brackets[i, 1]
					? Occurs(brackets[i, 0], line) % 2
					: Occurs(brackets[i, 0], line) - Occurs(brackets[i, 1], line);

				// Если в выражении есть хотя б один тип незакрытых скобок
				if (number != 0)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Проверить является ли символ печатным
		/// </summary>
		/// <param name="ch">Символ</param>
		/// <param name="includeNewLine">Считать символы перевода каретки печатными символами</param>
		/// <returns></returns>
		[Pure]
		public static bool IsPrintableCharacter(char ch, bool includeNewLine)
		{
			bool result = ch >= 32 || ch == '\t' || includeNewLine && (ch == '\n' || ch == '\r');
			return result;
		}

		/// <summary>
		/// Конвертировать число в строку и разбить цифры на триады
		/// </summary>
		[Pure]
		public static string SplitToTriads(int number)
		{
			string result = number.ToString("#,0", new NumberFormatInfo { NumberGroupSeparator = " " });
			return result;
		}
		
		/// <summary>
		/// Рассчитать процент схожести двух строк
		/// </summary>
		/// <param name="string1">Первая строка</param>
		/// <param name="string2">Вторая строка</param>
		/// <param name="caseSensitive">Чувствительно к регистру (<see langword="true"/>: a != A, <see langword="false"/>: a == A)</param>
		/// <returns>Действительное число (от 0 до 1.0) - процент схожести двух строк</returns>
		/// <exclude/>
		public static double SimilarText(string string1, string string2, bool caseSensitive = true)
		{
			if (string1 == null || string2 == null)
			{
				return 0d;
			}
			if (string1.Length == 0 || string2.Length == 0)
			{
				return 0d;
			}
			if (!caseSensitive)
			{
				string1 = string1.ToUpper();
				string2 = string2.ToUpper();
			}
			if (string1 == string2)
			{
				return 1d;
			}

			var stepsToSame = optimalStringAlignmentDistance(string1, string2);
			return (1.0d - stepsToSame / (double)Math.Max(string1.Length, string2.Length));
		}

		/// <summary>
		/// Редакционное расстояние между строками по алгоритму "optimal string alignment distance"
		/// </summary>
		/// <param name="string1">Первая строка</param>
		/// <param name="string2">Вторая строка</param>
		/// <returns>Целое число - количество шагов получиния из первой строки второй (или наоборот)</returns>
		private static int optimalStringAlignmentDistance(string string1, string string2)
		{
			if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
			{
				return Math.Max(string1 == null ? 0 : string1.Length, string2 == null ? 0 : string2.Length);
			}
			var length1 = string1.Length;
			var length2 = string2.Length;

			// Матрица расстояний (для любых i, j distances[i][j] = расстояние между первыми i символами string1 и j символами string2,
			// размер матрицы: (length1 + 1) x (length2 + 1))
			var distances = new int[length1 + 1][];
			for (var i = 0; i <= length1; i++)
			{
				distances[i] = new int[length2 + 1];
				distances[i][0] = i;
			}
			for (var j = 1; j <= length2; j++)
			{
				distances[0][j] = j;
			}

			// Заполнение матрицы расстояний
			for (var i = 1; i <= length1; i++)
			{
				for (var j = 1; j <= length2; j++)
				{
					var cost = string1[i - 1] == string2[j - 1] ? 0 : 1;
					distances[i][j] = Math.Min(distances[i][j - 1] + 1, // Вставка
						Math.Min(distances[i - 1][j] + 1,               // Удаление
						distances[i - 1][j - 1] + cost));               // Замена
																		// Транспозиция
					if (i > 1 && j > 1 && string1[i - 1] == string2[j - 2] && string1[i - 2] == string2[j - 1])
					{
						distances[i][j] = Math.Min(distances[i][j], distances[i - 2][j - 2] + cost);
					}
				}
			}
			return distances[length1][length2];
		}

		/// <summary>
		/// Сравнить две строки без учета регистра и правых пробелов
		/// </summary>
		/// <param name="str1">Первая строка для сравнения</param>
		/// <param name="str2">Вторая строка для сравнения</param>
		/// <returns>Результат сравнения</returns>
		[Pure]
		public static bool CompareEx(string str1, string str2)
		{
			str1 = trimEndIfNotNullOrEmpty(str1);
			str2 = trimEndIfNotNullOrEmpty(str2);
			return string.Compare(str1, str2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Обрезать строку справа, если она не null и не пуста
		/// </summary>
		/// <param name="str">Строка, которую нужно обрезать справа</param>
		/// <returns>Строка без крайних правых пробелов</returns>
		private static string trimEndIfNotNullOrEmpty(string str)
		{
			return String.IsNullOrEmpty(str) ? str : str.TrimEnd();
		}

		/// <summary>
		/// Дополнить строку слева до нужной длины пробелами или другим символом
		/// </summary>
		/// <param name="text">Текст</param>
		/// <param name="totalWidth">Требуемая длина. Если входная строка превышает <paramref name="totalWidth"/>, то она будет усечена</param>
		/// <param name="paddingChar">Символ, которым дополнить</param>
		/// <returns></returns>
		/// <remarks>Аналог VFP-функции padl()</remarks>
		[Pure]
		public static string PadLeft(string text, int totalWidth, char paddingChar = ' ')
		{
			return Left(text, totalWidth).PadLeft(totalWidth, paddingChar);
		}

		/// <summary>
		/// Дополнить строку справа до нужной длины пробелами или другим символом
		/// </summary>
		/// <param name="text">Текст</param>
		/// <param name="totalWidth">Требуемая длина. Если входная строка превышает <paramref name="totalWidth"/>, то она будет усечена</param>
		/// <param name="paddingChar">Символ, которым дополнить</param>
		/// <returns></returns>
		/// <remarks>Аналог VFP-функции padr()</remarks>
		[Pure]
		public static string PadRight(string text, int totalWidth, char paddingChar = ' ')
		{
			return Left(text, totalWidth).PadRight(totalWidth, paddingChar);
		}


		/// <summary>
		/// Центрировать строку
		/// </summary>
		/// <param name="stringToCenter">Строка, которую нужно разместить в центре</param>
		/// <param name="length">Длина результирующей строки</param>
		/// <returns>Центрирована строка</returns>
		[Pure]
		public static string PadCenter(string stringToCenter, int length)
		{
			stringToCenter = stringToCenter ?? string.Empty;
			// Если длина результирующей строки меньше длины строки, которую нужно центрировать, то вернуть первые length символов
			if (stringToCenter.Length > length)
			{
				return Left(stringToCenter, length);
			}

			stringToCenter = stringToCenter.PadLeft((length + stringToCenter.Length) / 2).PadRight(length);
			return stringToCenter;
		}

		/// <summary>
		/// Поиск позиции вхождения подстроки в строку по номеру вхождения
		/// </summary>
		/// <param name="inString">Строка, в которой осуществлять поиск</param>
		/// <param name="searchString">Строка поиска</param>
		/// <param name="occurs">Номер вхождения. Нумерация начинается с 1.</param>
		/// <returns>Номер позиции вхождения строки поиска в <see cref="inString"/>. Нумерация позиции начинается с 0.</returns>
		[Pure]
		public static int IndexOf(string inString, string searchString, int occurs)
		{
			int index = -1;
			int kol = 0;
			if (string.IsNullOrEmpty(inString) || string.IsNullOrEmpty(searchString))
			{
				return index;
			}
			while (true)
			{
				kol++;
				index = inString.IndexOf(searchString, index + 1, StringComparison.Ordinal);
				if (index < 0 || kol == occurs)
				{
					break;
				}
			}
			return index;
		}

		/// <summary>
		/// Извлечь подстроку из строки
		/// </summary>
		/// <param name="fullString">Строка из которой нужно извлечь подстроку</param>
		/// <param name="startPosition">Позиция первого знака подстроки в данной строке (с нуля)</param>
		/// <param name="length">Длина подстроки</param>
		/// <returns>Подстрока</returns>
		[Pure]
		public static string Substring(string fullString, int startPosition, int length)
		{
			if (String.IsNullOrEmpty(fullString))
			{
				return string.Empty;
			}
			int fullStringLength = fullString.Length;
			// Если позиция первого знака больше длины строки, или недопустимые значения позиции первого знака или длины,
			// то вернуть пустую строку
			if (startPosition > fullStringLength || startPosition < 0 || length <= 0)
			{
				return string.Empty;
			}

			// Если длина подстроки не задана, или подстрока выходит за пределы строки, то вернуть подстроку, начало которой в startPosition 
			if (length + startPosition > fullStringLength)
			{
				return fullString.Substring(startPosition);
			}
			return fullString.Substring(startPosition, length);
		}

		/// <summary>
		/// Извлечь подстроку из строки (от указанной позиции знака до конца строки)
		/// </summary>
		/// <param name="fullString">Строка из которой нужно извлечь подстроку</param>
		/// <param name="startPosition">Позиция первого знака подстроки в данной строке (с нуля)</param>
		/// <returns>Подстрока</returns>
		[Pure]
		public static string Substring(string fullString, int startPosition)
		{
			return Substring(fullString, startPosition, fullString.Length - startPosition);
		}

		/// <summary>
		/// Получить i-ю часть строки с разделителями
		/// </summary>
		/// <param name="expression">Входная строка, из которой нужно получить часть</param>
		/// <param name="index">Номер части строки (нумерация с единицы)</param>
		/// <param name="separator">Разделитель</param>		
		/// <returns>i-ая часть строки</returns>
		[Pure]
		public static string PartOfString(string expression, int index, string separator = ",")
		{
			if (String.IsNullOrEmpty(expression) || String.IsNullOrEmpty(separator) || index < 1)
			{
				return string.Empty;
			}
			// Перейти от нумерации с 1 до нумерации с 0
			var zeroBasedIndex = index - 1;
			// Разбить строку с разделителями на зоны
			var zones = expression.Split(new[] { separator }, StringSplitOptions.None);
			// Вернуть подстроку, если подстрока с переданным индексом существует. Иначе вернуть пустую строку
			if (zeroBasedIndex < zones.Length)
			{
				return zones[zeroBasedIndex];
			}
			return string.Empty;
		}

		/// <summary>
		/// Удалить лишние пробелы из строки (несколько подряд идущих пробелов заменяются на один)
		/// </summary>
		[Pure]
		public static string RemoveExtraSpaces(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}

			// находим группы символов между пробелами и склеиваем их в строку
			var returnValue = new StringBuilder();
			int wordPartBegin = 0;
			while (wordPartBegin < str.Length)
			{
				// группой символов будем считать подстроку, разделенную последовательностью из более чем 2 символов пробела
				// найти индекс конца следующей группы
				int wordPartEnd = wordPartBegin;
				// проверить, что на данной позиции нет подряд идущих двух пробелов
				while (wordPartEnd < str.Length && !(str[wordPartEnd] == ' ' && wordPartEnd + 1 < str.Length && str[wordPartEnd + 1] == ' '))
				{
					wordPartEnd++;
				}
				// добавить к выходной строке текущую группу
				returnValue.Append(str.Substring(wordPartBegin, wordPartEnd - wordPartBegin));
				// перейти к поиску следующей группы
				wordPartBegin = wordPartEnd + 1;
			}
			return returnValue.ToString();
		}

		/// <summary>
		/// Преобразовать текст в стиль Кэмел
		/// </summary>
		/// <param name="text">Текст для преобразования</param>
		/// <returns>Результат преобразования</returns>
		[Pure]
		public static string ToCamelCase(string text)
		{
			var result = new List<char>();
			// Признак, что следующая буква на верхнем регистре
			var upper = true;
			foreach (var ch in text)
			{
				// Если символ - подчеркивание или число,
				// то следующая буква должна быть на верхнем регистре
				if (ch == '_' || char.IsDigit(ch))
				{
					result.Add(ch);
					upper = true;
				}
				// Символы +,-,. заменяем на _
				else if (ch == '+' || ch == '.' || ch == '-')
				{
					result.Add('_');
				}
				// Переверсти букву на верхний регистр
				else if (upper)
				{
					result.Add(char.ToUpper(ch));
					upper = false;
				}
				// Просто добавить символ
				else
				{
					result.Add(char.ToLower(ch));
				}
			}
			return new string(result.ToArray());
		}

		/// <summary>
		/// Заменить в строке все вхождения подстроки на другую строку
		/// </summary>
		/// <param name="str">Исходная строка</param>
		/// <param name="oldValue">Заменяемая строка</param>
		/// <param name="newValue">Строка для подстановки</param>
		/// <param name="caseSensitive">С учетом регистра</param>
		/// <returns></returns>
		public static string Replace(string str, string oldValue, string newValue, bool caseSensitive = true)
		{
			// Защита от null
			if (string.IsNullOrWhiteSpace(str))
			{
				return string.Empty;
			}

			// Ничего не меняем
			if (string.IsNullOrEmpty(oldValue) || newValue == null)
			{
				return str;
			}

			// Если необходить анализировать регистр - вызывать стандартный метод
			if (caseSensitive)
			{
				return str.Replace(oldValue, newValue);
			}

			int position0, position1;
			var count = position0 = 0;

			var upperString = str.ToUpperInvariant();
			var upperPattern = oldValue.ToUpperInvariant();

			// Определить максимальную длину строки
			var inc = (str.Length / oldValue.Length) * (newValue.Length - oldValue.Length);
			var chars = new char[str.Length + Math.Max(0, inc)];

			// Пока есть вхождения заменяемой строки в исходной (замена не выполняется, изменяется индекс начала поиска)
			while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.CurrentCultureIgnoreCase)) != -1)
			{
				for (var i = position0; i < position1; ++i)
				{
					chars[count++] = str[i];
				}
				foreach (var c in newValue)
				{
					chars[count++] = c;
				}
				position0 = position1 + oldValue.Length;
			}
			// Если не выполнено ни одной замены - вернуть исходную строку
			if (position0 == 0)
			{
				return str;
			}
			for (var i = position0; i < str.Length; ++i)
			{
				chars[count++] = str[i];
			}
			return new string(chars, 0, count);
		}

		/// <summary>
		/// Заменить все вхождения слова.
		/// Выполняется замена только целого слова. Если вхождение является частью другого слова, то замена не выполняется.
		/// </summary>
		/// <param name="str">Строка, в которой ищутся вхождения</param>
		/// <param name="word">Слово для замены</param>
		/// <param name="replacement">Слово, которым заменять</param>
		/// <returns>Результат замены</returns>
		public static string ReplaceWord(string str, string word, string replacement)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			var occurs = new List<int>();
			int index = 0;
			var wordLen = word.Length;
			// Найти все вхождения слова
			do
			{
				index = str.IndexOf(word, index, StringComparison.Ordinal);
				if (index < 0)
				{
					break;
				}
				// Проверяем допустимость символа слева и справа
				var endIndex = index + wordLen;
				if ((index == 0 || !IsPartOfWord(str[index - 1])) &&
					(endIndex == str.Length || !IsPartOfWord(str[endIndex])))
				{
					// Нашли полное слово - выполняем замену
					occurs.Add(index);
					index = endIndex;
				}
				else
				{
					index++;
				}

			} while (true);
			// Выполнить замены
			if (occurs.Count > 0)
			{
				var sb = new StringBuilder(str);
				for (int i = occurs.Count - 1; i >= 0; i--)
				{
					var occurence = occurs[i];
					sb.Remove(occurence, wordLen);
					sb.Insert(occurence, replacement);
				}
				str = sb.ToString();
			}
			return str;
		}

		/// <summary>
		/// Падеж
		/// </summary>
		public enum Case
		{
			/// <summary>
			/// Именительный падеж
			/// </summary>
			Nominative = 1,

			/// <summary>
			/// Родительный падеж
			/// </summary>
			Genitive = 2,

			/// <summary>
			/// Дательный падеж
			/// </summary>
			Dative = 3,

			/// <summary>
			/// Винительный падеж
			/// </summary>
			Accusative = 4
		}
	}
}
