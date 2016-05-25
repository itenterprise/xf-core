using System;
using System.Collections.Generic;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.WebService;
using Common.CoreUWP.Providers;
using Xamarin.Forms;

[assembly: Dependency(typeof(Utility))]
namespace Common.CoreUWP.Providers
{
	public class Utility : UtilityBase
	{
		public override IEnumerable<Account> FindAccountsForService(string serverUrl)
		{
			return new List<Account>();
		}

		public override void Save(Account account)
		{
		}

		public override void Delete(string username, string serverUrl)
		{
		}
	}
}