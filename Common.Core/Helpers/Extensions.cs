using System.Collections.Generic;
using System.Text;

namespace Common.Core.Helpers
{
	public static class Extensions
	{
		public static string GetFileUrl(this string fileName)
		{
			return string.Concat(WebServiceHelper.GetFileUrl(fileName), "&nodownload=1");
		}

		public static string GetWidgetUrl(this string widgetId, Dictionary<string, string> query = null)
		{
			var link = WebServiceHelper.GetWidgetUrl(widgetId, Settings.Ticket);
			query = query ?? new Dictionary<string, string>();
			if (!query.ContainsKey("itlanguage"))
			{
				query.Add("itlanguage", Settings.Culture);
			}
			if (!query.ContainsKey("devexpress_theme"))
			{
				query.Add("devexpress_theme", "moderno");
			}
			var linkBuilder = new StringBuilder(link);
			foreach (var param in query)
			{
				linkBuilder.AppendFormat("&{0}={1}", param.Key, param.Value);
			}
			return linkBuilder.ToString();
		}

		public static string WrapLinkByFrame(this string link)
		{
			return $"<html><iframe src = \"{link}\" width=\"100%\" height=\"1200\" frameborder=\"no\" hspace=\"0\" vspace=\"0\" /></html>";
		}
	}
}