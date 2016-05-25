using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.WebService;
using Common.CoreDroid.Providers;
using Java.IO;
using Java.Security;
using Javax.Crypto;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(Utility))]

namespace Common.CoreDroid.Providers
{
	/// <summary>
	/// AccountStore that uses a KeyStore of PrivateKeys protected by a fixed password
	/// in a private region of internal storage.
	/// </summary>
	internal class Utility : UtilityBase
	{
		private readonly Context _context;
		private readonly KeyStore _ks;
		private readonly KeyStore.PasswordProtection _prot;

		private static readonly object _fileLock = new object();

		const string _fileName = "ITEnterprise.Accounts";
		static readonly char[] _password = "3295043EA18CA264B2C40E0B72051DEF2D07AD2B4593F43DDDE1515A7EC32617".ToCharArray();

		public Utility()
		{
			_context = Application.Context;
			_ks = KeyStore.GetInstance(KeyStore.DefaultType);
			_prot = new KeyStore.PasswordProtection(_password);
			try
			{
				lock (_fileLock)
				{
					using (var s = _context.OpenFileInput(_fileName))
					{
						_ks.Load(s, _password);
					}
				}
			}
			catch (FileNotFoundException)
			{
				//ks.Load (null, Password);
				loadEmptyKeyStore(_password);
			}
		}

		public override IEnumerable<Account> FindAccountsForService(string serverUrl)
		{
			var r = new List<Account>();

			var postfix = "-" + serverUrl;

			var aliases = _ks.Aliases();
			while (aliases.HasMoreElements)
			{
				var alias = aliases.NextElement().ToString();
				if (alias.EndsWith(postfix))
				{
					var e = _ks.GetEntry(alias, _prot) as KeyStore.SecretKeyEntry;
					if (e != null)
					{
						var bytes = e.SecretKey.GetEncoded();
						var serialized = System.Text.Encoding.UTF8.GetString(bytes);
						var acct = SerializationHelper.Deserialize<Account>(serialized);
						r.Add(acct);
					}
				}
			}

			r.Sort((a, b) => string.Compare(a.Login, b.Login, StringComparison.Ordinal));

			return r;
		}

		public override void Save(Account account)
		{
			account.Login = account.Login.ToUpper();
			var alias = makeAlias(account.Login, account.ServerUrl);

			var secretKey = new SecretAccount(account);
			var entry = new KeyStore.SecretKeyEntry(secretKey);
			_ks.SetEntry(alias, entry, _prot);

			save();
		}

		public override void Delete(string username, string serverUrl)
		{
			try
			{
				username = username.ToUpper();
				var alias = makeAlias(username, serverUrl);

				_ks.DeleteEntry(alias);
				save();
			}
			catch(Exception e)
			{
			}
		}

		private void save()
		{
			lock (_fileLock)
			{
				using (var s = _context.OpenFileOutput(_fileName, FileCreationMode.Private))
				{
					_ks.Store(s, _password);
				}
			}
		}

		private static string makeAlias(string username, string serviceId)
		{
			return username + "-" + serviceId;
		}

		class SecretAccount : Java.Lang.Object, ISecretKey
		{
			readonly byte[] _bytes;
			public SecretAccount(Account account)
			{
				_bytes = System.Text.Encoding.UTF8.GetBytes(SerializationHelper.Serialize(account));
			}
			public byte[] GetEncoded()
			{
				return _bytes;
			}
			public string Algorithm
			{
				get
				{
					return "RAW";
				}
			}
			public string Format
			{
				get
				{
					return "RAW";
				}
			}
		}

		static IntPtr _idLoadLjavaIoInputStreamArrayC;

		/// <summary>
		/// Work around Bug https://bugzilla.xamarin.com/show_bug.cgi?id=6766
		/// </summary>
		void loadEmptyKeyStore(char[] password)
		{
			if (_idLoadLjavaIoInputStreamArrayC == IntPtr.Zero)
			{
				_idLoadLjavaIoInputStreamArrayC = JNIEnv.GetMethodID(_ks.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = JNIEnv.NewArray(password);
			JNIEnv.CallVoidMethod(_ks.Handle, _idLoadLjavaIoInputStreamArrayC, new JValue (intPtr), new JValue (intPtr2));
			JNIEnv.DeleteLocalRef(intPtr);
			if (password != null)
			{
				JNIEnv.CopyArray(intPtr2, password);
				JNIEnv.DeleteLocalRef(intPtr2);
			}
		}
	}
}