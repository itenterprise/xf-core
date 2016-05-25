namespace Common.Core.Interfaces
{
	public interface ISkypeProvider
	{
		bool IsInstalled();
		void Open(string login, SkypeAction action);
	}

	public enum SkypeAction
	{
		Call,
		Chat
	};
}
