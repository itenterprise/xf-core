using Newtonsoft.Json;

namespace Common.Core.WebService
{
	public class Account
	{
		[JsonProperty("LOGIN")]
		public string Login { get; set; }


		[JsonProperty("PASSWORD")]
		public string Password { get; set; }


		[JsonProperty("SERVERURL")]
		public string ServerUrl { get; set; }
	}
}