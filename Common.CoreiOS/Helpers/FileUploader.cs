using System.Text;
using System.Threading.Tasks;
using Common.Core.Helpers;
using Common.CoreiOS.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(FileUploader))]
namespace Common.CoreiOS.Helpers
{
	public class FileUploader : IFileUploader
	{
		public async Task<string> UploadFile(string fileName)
		{
			var client = new System.Net.WebClient();
			client.Headers.Add("Content-Type", "binary/octet-stream");
			var result = await client.UploadFileTaskAsync(Settings.Url.Replace("webservice.asmx", "addfile.ashx"), "POST", fileName);
			return Encoding.UTF8.GetString(result, 0, result.Length);
		}
	}
}
