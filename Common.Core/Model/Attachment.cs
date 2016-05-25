using System;
using Common.Core.Tools;
using Newtonsoft.Json;

/// <summary>
/// Описание вложения
/// </summary>
public class Attachment
{
	/// <summary>
	/// Уникальный номер
	/// </summary>
	[JsonProperty("NDOR")]
	public int Ndor { get; set; }

	/// <summary>
	/// Имя файла
	/// </summary>
	[JsonProperty("FILENAME")]
	public string FileName { get; set; }

	/// <summary>
	/// ФИО пользователя, который добавил
	/// </summary>
	[JsonProperty("USER")]
	public string User { get; set; }

	/// <summary>
	/// Дата добавления
	/// </summary>
	[JsonProperty("DateTime")]
	public DateTime DateTimeAdd { get; set; }

	/// <summary>
	/// Признак владения
	/// </summary>
	[JsonProperty("ISMY")]
	public bool IsMy { get; set; }

	/// <summary>
	/// Расширение файла
	/// </summary>
	[JsonProperty("FILEEXT")]
	public string FileExt { get; set; }
}