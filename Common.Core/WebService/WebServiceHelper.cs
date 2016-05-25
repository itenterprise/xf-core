using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Common.Core.Helpers;
using Common.Core.Properties;
using Common.Core.Tools;
using Xamarin.Forms;

namespace Common.Core
{
	/// <summary>
	/// Класс для запуска веб-расчетов
	/// </summary>
	public static class WebServiceHelper
	{
		public static event Action<TicketExpiredEventArgs> OnTicketExpired;

		public static bool ShowConnectionErrorMessage { get; set; }

		private static void onTicketExpired(TicketExpiredEventArgs args)
		{
			if (OnTicketExpired != null)
			{
				OnTicketExpired(args);
			}
		}

		public class ServiceCallResult
		{
			public ResultStatus Success { get; set; }
		}

		public class TicketExpiredEventArgs
		{
			public bool NewTokenTaken { get; set; }
		}

		/// <summary>
		/// Выполнить расчет
		/// </summary>
		public static async void ExecuteNonQuery(string serviceName, object parameters)
		{
			await ExecuteWithReconnect(serviceName, parameters);
		}

		/// <summary>
		/// Выполнить расчет
		/// </summary>
		public static async Task<string> Execute(string serviceName, object parameters)
		{
			var json = await ExecuteWithReconnect(serviceName, parameters);
			return json ?? string.Empty;
		}

		/// <summary>
		/// Выполнить расчет
		/// </summary>
		public static async Task<TResult> Execute<TResult>(string serviceName, object parameters)
		{
            var json = await ExecuteWithReconnect(serviceName, parameters);
			return string.IsNullOrEmpty(json)
				? Activator.CreateInstance<TResult>()
				: JsonConvert.DeserializeObject<TResult>(json);
		}

		/// <summary>
		/// Выполнить расчет.
		/// Если произошли ошибки подключения - повторить попытку до 3-х раз
		/// </summary>
		internal static async Task<string> ExecuteWithReconnect(string serviceName, object parameters, int trys = 0)
		{
			while (trys < 3)
			{
				var success = true;
				string responseStr = null;
				try
				{
					var argsString = JsonConvert.SerializeObject(parameters);
					var index = 0;
					while (argsString.Substring(index).Contains("\"Date"))
					{
						var begin = argsString.IndexOf("\"Date", index, StringComparison.Ordinal);
						var end = argsString.IndexOf("\"", begin + 1, StringComparison.Ordinal);
						argsString = argsString.Insert(end, "\\/");
						argsString = argsString.Insert(begin + 1, "\\/");
						index = end;
					}

					var parametersObject = new
					{
						calcId = serviceName,
						args = argsString,
						ticket = Settings.Ticket
					};

					var httpClient = new HttpClient();
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					var urlParts = Settings.Url.Split('?');
					var req = new HttpRequestMessage(HttpMethod.Post, new Uri(urlParts[0] + "/ExecuteEx?" + (urlParts.Length > 1 ? urlParts[1] : string.Empty) + "&pureJson="))
					{
						Headers = { AcceptLanguage = { new StringWithQualityHeaderValue(Settings.Culture) } },
						Content = new StringContent(JsonConvert.SerializeObject(parametersObject), Encoding.UTF8, "application/json")
					};
					var response = await httpClient.SendAsync(req);
					responseStr = await response.Content.ReadAsStringAsync();
				}
				catch (Exception)
				{
					success = false;
				}
				if (success && string.IsNullOrEmpty(responseStr))
				{
					success = false;
				}
				if (success)
				{
					return await prepareResult(responseStr, serviceName, parameters, trys);
				}
				trys = trys + 1;
			}
			return string.Empty;
		}

		public async static Task<string> SendFile(string fileName)
		{
			return await DependencyService.Get<IFileUploader>().UploadFile(fileName);
		}

		private async static Task<string> prepareResult(string jsonResult, string serviceName, object parameters, int trys)
		{
			if (jsonResult == "WRONG_TICKET")
			{
				Settings.Ticket = null;
				var args = new TicketExpiredEventArgs();
				onTicketExpired(args);
				if (args.NewTokenTaken)
				{
					return await ExecuteWithReconnect(serviceName, parameters, trys);
				}
				Resources.CannotConnectToServer.ToToast();
				return string.Empty;
			}
			if (jsonResult == "Calculation not exists")
			{
				Resources.CannotFindWebCalculation.ToToast();
				return string.Empty;
			}
			return jsonResult;
		}

		public static string GetFileUrl(string fileName)
		{
			return $"{getBaseUrl()}GetFile.ashx?file={fileName}&folder=content";
		}

		public static string GetWidgetUrl(string widgetId, string ticket)
		{
			return $"{getBaseUrl()}webparts/?id={widgetId}&authticket={ticket}".Replace("/ws", "");
		}

		private static string getUrl()
		{
			var url = Settings.Url;
			if (url.EndsWith("/"))
			{
				return url;
			}
			return url + "/";
		}

		private static string getBaseUrl()
		{
			var url = getUrl().ToLower();
			url = url.Substring(0, url.IndexOf("webservice.asmx", StringComparison.Ordinal));
			if (url.EndsWith("/"))
			{
				return url;
			}
			return url + "/";
		}
	}

	/// <summary>
	/// Результат расчета
	/// </summary>
	public enum ResultStatus
	{
		/// <summary>
		/// Выполнен успешно
		/// </summary>
		Success,

		/// <summary>
		/// Не выполнен
		/// </summary>
		Fail,

		/// <summary>
		/// Нет связи
		/// </summary>
		NoNetwork
	}
}