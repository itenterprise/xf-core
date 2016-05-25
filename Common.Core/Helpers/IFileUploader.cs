using System.Threading.Tasks;

namespace Common.Core.Helpers
{
	public interface IFileUploader
	{
		Task<string> UploadFile(string fileName);
	}
}
