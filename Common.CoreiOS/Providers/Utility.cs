using System;
using System.Collections.Generic;
using System.Linq;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.WebService;
using Common.CoreiOS.Providers;
using Foundation;
using Security;
using Xamarin.Forms;

[assembly: Dependency(typeof(Utility))]
namespace Common.CoreiOS.Providers
{
	public class Utility : UtilityBase
	{
		public override IEnumerable<Account> FindAccountsForService(string serverUrl)
		{
			var query = new SecRecord(SecKind.GenericPassword) {
				Service = serverUrl
			};

			SecStatusCode result;
			var records = SecKeyChain.QueryAsRecord(query, 1000, out result);

			return records != null ?
				records.Select(getAccountFromRecord).ToList() :
				new List<Account>();
		}

		private static Account getAccountFromRecord(SecRecord r)
		{
			var serializedData = NSString.FromData(r.Generic, NSStringEncoding.UTF8);
			return SerializationHelper.Deserialize<Account>(serializedData);
		}

		private static Account findAccount(string username, string serviceId)
		{
			var query = new SecRecord(SecKind.GenericPassword) {
				Service = serviceId,
				Account = username
			};

			SecStatusCode result;
			var record = SecKeyChain.QueryAsRecord(query, out result);

			return record != null ? getAccountFromRecord(record) : null;
		}

		public override void Save(Account account)
		{
			account.Login = account.Login.ToUpper();
			SecStatusCode statusCode;
			var serializedAccount = SerializationHelper.Serialize(account);
			var data = NSData.FromString(serializedAccount, NSStringEncoding.UTF8);

			//
			// Remove any existing record
			//
			var existing = findAccount(account.Login, account.ServerUrl);

			if (existing != null)
			{
				var query = new SecRecord(SecKind.GenericPassword) {
					Service = account.ServerUrl,
					Account = account.Login
				};

				statusCode = SecKeyChain.Remove(query);
				if (statusCode != SecStatusCode.Success)
				{
					throw new Exception("Could not save account to KeyChain: " + statusCode);
				}
			}

			//
			// Add this record
			//
			var record = new SecRecord(SecKind.GenericPassword) {
				Service = account.ServerUrl,
				Account = account.Login,
				Generic = data,
				Accessible = SecAccessible.WhenUnlocked
			};

			statusCode = SecKeyChain.Add(record);

			if (statusCode != SecStatusCode.Success)
			{
				throw new Exception("Could not save account to KeyChain: " + statusCode);
			}
		}

		public override void Delete(string username, string serverUrl)
		{
			username = username.ToUpper();
			var query = new SecRecord(SecKind.GenericPassword) {
				Service = serverUrl,
				Account = username
			};

			var statusCode = SecKeyChain.Remove(query);

			if (statusCode != SecStatusCode.Success)
			{
				//throw new Exception("Could not delete account from KeyChain: " + statusCode);
			}
		}

	}
}