using System.Collections.Generic;
using System.Linq;
using Common.Core.Tools;
using Common.Core.WebService;

namespace Common.Core.Interfaces
{
	public interface IUtility
	{
		void Save(Account account);
		Account FindAccountForService(string username, string serverUrl);
		Account FindAccountForService(string serverUrl);
		IEnumerable<Account> FindAccountsForService(string serverUrl);
		void Delete(string username, string serverUrl);
	}

	public abstract class UtilityBase : IUtility
	{
		public Account FindAccountForService(string username, string serverUrl)
		{
			var accounts = FindAccountsForService(serverUrl);
			if (accounts == null)
			{
				return null;
			}
			return accounts.FirstOrDefault(account => Text.CompareEx(account.Login, username));
		}

		public Account FindAccountForService(string serverUrl)
		{
			var accounts = FindAccountsForService(serverUrl);
			if (accounts == null)
			{
				return null;
			}
			return accounts.FirstOrDefault();
		}

		public abstract void Save(Account account);

		public abstract IEnumerable<Account> FindAccountsForService(string serverUrl);

		public abstract void Delete(string username, string serverUrl);
	}
}