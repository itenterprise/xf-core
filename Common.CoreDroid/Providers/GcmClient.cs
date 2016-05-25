using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")] //, ProtectionLevel = Android.Content.PM.Protection.Signature)]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
namespace Common.CoreDroid.Providers
{
	public class Logger
	{
		public static bool Enabled = false;

		public static void Debug(string msg)
		{
			if (Enabled)
				Android.Util.Log.Debug("GCM-CLIENT", msg);
		}
	}

	public class Constants
	{
		public const string INTENT_TO_GCM_REGISTRATION = "com.google.android.c2dm.intent.REGISTER";

		/*		*
		* Intent sent to GCM to unregister the application.
		*/
		public const string INTENT_TO_GCM_UNREGISTRATION = "com.google.android.c2dm.intent.UNREGISTER";

		/*		*
		* Intent sent by GCM indicating with the result of a registration request.
		*/
		public const string INTENT_FROM_GCM_REGISTRATION_CALLBACK = "com.google.android.c2dm.intent.REGISTRATION";

		/*		*
		* Intent used by the GCM library to indicate that the registration call
		* should be retried.
		*/
		public const string INTENT_FROM_GCM_LIBRARY_RETRY = "com.google.android.gcm.intent.RETRY";

		/*		*
		* Intent sent by GCM containing a message.
		*/
		public const string INTENT_FROM_GCM_MESSAGE = "com.google.android.c2dm.intent.RECEIVE";

		/*		*
		* Extra used on {@link #INTENT_TO_GCM_REGISTRATION} to indicate the sender
		* account (a Google email) that owns the application.
		*/
		public const string EXTRA_SENDER = "sender";

		/*		*
		* Extra used on {@link #INTENT_TO_GCM_REGISTRATION} to get the application
		* id.
		*/
		public const string EXTRA_APPLICATION_PENDING_INTENT = "app";

		/*		*
		* Extra used on {@link #INTENT_FROM_GCM_REGISTRATION_CALLBACK} to indicate
		* that the application has been unregistered.
		*/
		public const string EXTRA_UNREGISTERED = "unregistered";

		/*		*
		* Extra used on {@link #INTENT_FROM_GCM_REGISTRATION_CALLBACK} to indicate
		* an error when the registration fails. See constants starting with ERROR_
		* for possible values.
		*/
		public const string EXTRA_ERROR = "error";

		/*		*
		* Extra used on {@link #INTENT_FROM_GCM_REGISTRATION_CALLBACK} to indicate
		* the registration id when the registration succeeds.
		*/
		public const string EXTRA_REGISTRATION_ID = "registration_id";

		/*		*
		* Type of message present in the {@link #INTENT_FROM_GCM_MESSAGE} intent.
		* This extra is only set for special messages sent from GCM, not for
		* messages originated from the application.
		*/
		public const string EXTRA_SPECIAL_MESSAGE = "message_type";

		/*		*
		* Special message indicating the server deleted the pending messages.
		*/
		public const string VALUE_DELETED_MESSAGES = "deleted_messages";

		/*		*
		* Number of messages deleted by the server because the device was idle.
		* Present only on messages of special type
		* {@link #VALUE_DELETED_MESSAGES}
		*/
		public const string EXTRA_TOTAL_DELETED = "total_deleted";

		/*		*
		* Permission necessary to receive GCM intents.
		*/
		public const string PERMISSION_GCM_INTENTS = "com.google.android.c2dm.permission.SEND";

		/*		*
		* @see GCMBroadcastReceiver
		*/
		public const string DEFAULT_INTENT_SERVICE_CLASS_NAME = ".GCMIntentService";

		/*		*
		* The device can't read the response, or there was a 500/503 from the
		* server that can be retried later. The application should use exponential
		* back off and retry.
		*/
		public const string ERROR_SERVICE_NOT_AVAILABLE = "SERVICE_NOT_AVAILABLE";

		/*		*
		* There is no Google account on the phone. The application should ask the
		* user to open the account manager and add a Google account.
		*/
		public const string ERROR_ACCOUNT_MISSING = "ACCOUNT_MISSING";

		/*		*
		* Bad password. The application should ask the user to enter his/her
		* password, and let user retry manually later. Fix on the device side.
		*/
		public const string ERROR_AUTHENTICATION_FAILED = "AUTHENTICATION_FAILED";

		/*		*
		* The request sent by the phone does not contain the expected parameters.
		* This phone doesn't currently support GCM.
		*/
		public const string ERROR_INVALID_PARAMETERS = "INVALID_PARAMETERS";
		/*		*
		* The sender account is not recognized. Fix on the device side.
		*/
		public const string ERROR_INVALID_SENDER = "INVALID_SENDER";

		/*		*
		* Incorrect phone registration with Google. This phone doesn't currently
		* support GCM.
		*/
		public const string ERROR_PHONE_REGISTRATION_ERROR = "PHONE_REGISTRATION_ERROR";

	}

	public class GcmClient
	{
		const string BACKOFF_MS = "backoff_ms";
		const string GSF_PACKAGE = "com.google.android.gsf";
		const string PREFERENCES = "com.google.android.gcm";
		const int DEFAULT_BACKOFF_MS = 3000;
		const string PROPERTY_REG_ID = "regId";
		const string PROPERTY_APP_VERSION = "appVersion";
		const string PROPERTY_ON_SERVER = "onServer";

		//static GCMBroadcastReceiver sRetryReceiver;

		public static void CheckDevice(Context context)
		{
			var version = (int)Build.VERSION.SdkInt;
			if (version < 8)
				throw new InvalidOperationException("Device must be at least API Level 8 (instead of " + version + ")");

			var packageManager = context.PackageManager;

			try
			{
				packageManager.GetPackageInfo(GSF_PACKAGE, 0);
			}
			catch
			{
				throw new InvalidOperationException("Device does not have package " + GSF_PACKAGE);
			}
		}

		public static void CheckManifest(Context context)
		{
			var packageManager = context.PackageManager;
			var packageName = context.PackageName;
			var permissionName = packageName + ".permission.C2D_MESSAGE";

			if (string.IsNullOrEmpty(packageName))
				throw new NotSupportedException("Your Android app must have a package name!");

			if (char.IsUpper(packageName[0]))
				throw new NotSupportedException("Your Android app package name MUST start with a lowercase character.  Current Package Name: " + packageName);

			try
			{
				packageManager.GetPermissionInfo(permissionName, PackageInfoFlags.Permissions);
			}
			catch
			{
				throw new AccessViolationException("Application does not define permission: " + permissionName);
			}

			PackageInfo receiversInfo;

			try
			{
				receiversInfo = packageManager.GetPackageInfo(packageName, PackageInfoFlags.Receivers);
			}
			catch
			{
				throw new InvalidOperationException("Could not get receivers for package " + packageName);
			}

			var receivers = receiversInfo.Receivers;

			if (receivers == null || receivers.Count <= 0)
				throw new InvalidOperationException("No Receiver for package " + packageName);

			Logger.Debug("number of receivers for " + packageName + ": " + receivers.Count);

			var allowedReceivers = new HashSet<string>();

			foreach (var receiver in receivers)
			{
				if (Constants.PERMISSION_GCM_INTENTS.Equals(receiver.Permission))
					allowedReceivers.Add(receiver.Name);
			}

			if (allowedReceivers.Count <= 0)
				throw new InvalidOperationException("No receiver allowed to receive " + Constants.PERMISSION_GCM_INTENTS);

			CheckReceiver(context, allowedReceivers, Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK);
			CheckReceiver(context, allowedReceivers, Constants.INTENT_FROM_GCM_MESSAGE);
		}

		private static void CheckReceiver(Context context, HashSet<string> allowedReceivers, string action)
		{
			var pm = context.PackageManager;
			var packageName = context.PackageName;

			var intent = new Intent(action);
			intent.SetPackage(packageName);

			var receivers = pm.QueryBroadcastReceivers(intent, PackageInfoFlags.IntentFilters);

			if (receivers == null || receivers.Count <= 0)
				throw new InvalidOperationException("No receivers for action " + action);

			Logger.Debug("Found " + receivers.Count + " receivers for action " + action);

			foreach (var receiver in receivers)
			{
				var name = receiver.ActivityInfo.Name;
				if (!allowedReceivers.Contains(name))
					throw new InvalidOperationException("Receiver " + name + " is not set with permission " + Constants.PERMISSION_GCM_INTENTS);
			}
		}

		public static void Register(Context context, params string[] senderIds)
		{
			SetRetryBroadcastReceiver(context);
			ResetBackoff(context);

			internalRegister(context, senderIds);
		}

		internal static void internalRegister(Context context, params string[] senderIds)
		{
			if (senderIds == null || senderIds.Length <= 0)
				throw new ArgumentException("No senderIds");

			var senders = string.Join(",", senderIds);

			Logger.Debug("Registering app " + context.PackageName + " of senders " + senders);

			var intent = new Intent(Constants.INTENT_TO_GCM_REGISTRATION);
			intent.SetPackage(GSF_PACKAGE);
			intent.PutExtra(Constants.EXTRA_APPLICATION_PENDING_INTENT,
				PendingIntent.GetBroadcast(context, 0, new Intent(), 0));
			intent.PutExtra(Constants.EXTRA_SENDER, senders);

			context.StartService(intent);
		}

		public static void UnRegister(Context context)
		{
			SetRetryBroadcastReceiver(context);
			ResetBackoff(context);
			internalUnRegister(context);
		}

		internal static void internalUnRegister(Context context)
		{
			Logger.Debug("Unregistering app " + context.PackageName);

			var intent = new Intent(Constants.INTENT_TO_GCM_UNREGISTRATION);
			intent.SetPackage(GSF_PACKAGE);
			intent.PutExtra(Constants.EXTRA_APPLICATION_PENDING_INTENT,
				PendingIntent.GetBroadcast(context, 0, new Intent(), 0));

			context.StartService(intent);
		}

		static void SetRetryBroadcastReceiver(Context context)
		{
			return;

			/*			if (sRetryReceiver == null)
			{
				sRetryReceiver = new GCMBroadcastReceiver();
				var category = context.PackageName;
				var filter = new IntentFilter(GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY);
				filter.AddCategory(category);
				var permission = category + ".permission.C2D_MESSAGE";
				Log.Verbose(TAG, "Registering receiver");
				context.RegisterReceiver(sRetryReceiver, filter, permission, null);
			}*/
		}

		public static string GetRegistrationId(Context context)
		{
			var prefs = GetGCMPreferences(context);

			var registrationId = prefs.GetString(PROPERTY_REG_ID, "");

			int oldVersion = prefs.GetInt(PROPERTY_APP_VERSION, int.MinValue);
			int newVersion = GetAppVersion(context);

			if (oldVersion != int.MinValue && oldVersion != newVersion)
			{
				Logger.Debug("App version changed from " + oldVersion + " to " + newVersion + "; resetting registration id");

				ClearRegistrationId(context);
				registrationId = string.Empty;
			}

			return registrationId;
		}

		public static bool IsRegistered(Context context)
		{
			var registrationId = GetRegistrationId(context);

			return !string.IsNullOrEmpty(registrationId);
		}

		internal static string ClearRegistrationId(Context context)
		{
			return SetRegistrationId(context, "");
		}

		internal static string SetRegistrationId(Context context, string registrationId)
		{
			var prefs = GetGCMPreferences(context);

			var oldRegistrationId = prefs.GetString(PROPERTY_REG_ID, "");
			int appVersion = GetAppVersion(context);
			Logger.Debug("Saving registrationId on app version " + appVersion);
			var editor = prefs.Edit();
			editor.PutString(PROPERTY_REG_ID, registrationId);
			editor.PutInt(PROPERTY_APP_VERSION, appVersion);
			editor.Commit();
			return oldRegistrationId;
		}


		public static void SetRegisteredOnServer(Context context, bool flag)
		{
			var prefs = GetGCMPreferences(context);
			Logger.Debug("Setting registered on server status as: " + flag);
			var editor = prefs.Edit();
			editor.PutBoolean(PROPERTY_ON_SERVER, flag);
			editor.Commit();
		}

		public static bool IsRegisteredOnServer(Context context)
		{
			var prefs = GetGCMPreferences(context);
			bool isRegistered = prefs.GetBoolean(PROPERTY_ON_SERVER, false);
			Logger.Debug("Is registered on server: " + isRegistered);
			return isRegistered;
		}

		static int GetAppVersion(Context context)
		{
			try
			{
				var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
				return packageInfo.VersionCode;
			}
			catch
			{
				throw new InvalidOperationException("Could not get package name");
			}
		}

		internal static void ResetBackoff(Context context)
		{
			Logger.Debug("resetting backoff for " + context.PackageName);
			SetBackoff(context, DEFAULT_BACKOFF_MS);
		}

		internal static int GetBackoff(Context context)
		{
			var prefs = GetGCMPreferences(context);
			return prefs.GetInt(BACKOFF_MS, DEFAULT_BACKOFF_MS);
		}

		internal static void SetBackoff(Context context, int backoff)
		{
			var prefs = GetGCMPreferences(context);
			var editor = prefs.Edit();
			editor.PutInt(BACKOFF_MS, backoff);
			editor.Commit();
		}

		static ISharedPreferences GetGCMPreferences(Context context)
		{
			return context.GetSharedPreferences(PREFERENCES, FileCreationMode.Private);
		}
	}

	public abstract class GcmBroadcastReceiverBase<TIntentService> : WakefulBroadcastReceiver where TIntentService : GcmServiceBase
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Logger.Debug("OnReceive: " + intent.Action);
			var className = context.PackageName + Constants.DEFAULT_INTENT_SERVICE_CLASS_NAME;

			Logger.Debug("GCM IntentService Class: " + className);

			GcmServiceBase.RunIntentInService(context, intent, typeof(TIntentService));
			SetResult(Result.Ok, null, null);
		}
	}

	[Android.Runtime.Preserve(AllMembers = true)]
	public abstract class GcmServiceBase : IntentService
	{
		const string WAKELOCK_KEY = "GCM_LIB";
		static PowerManager.WakeLock sWakeLock;

		static object LOCK = new object();
		static int serviceId = 1;

		/// <summary>
		/// The GCM Sender Ids to use. Set by the constructor taking parameters but not by the one that doesn't. Be very careful changing this value, preferably only set it in your constructor and only once.
		/// </summary>
		protected string[] SenderIds = new string[] { };

		//int sCounter = 1;
		Random sRandom = new Random();

		const int MAX_BACKOFF_MS = 3600000; //1 hour

		string TOKEN = "";
		const string EXTRA_TOKEN = "token";

		protected GcmServiceBase() : base() { }

		public GcmServiceBase(params string[] senderIds)
			: base("GCMIntentService-" + (serviceId++))
		{
			SenderIds = senderIds;
		}


		protected abstract void OnMessage(Context context, Intent intent);

		protected virtual void OnDeletedMessages(Context context, int total)
		{
		}

		protected virtual bool OnRecoverableError(Context context, string errorId)
		{
			return true;
		}

		protected abstract void OnError(Context context, string errorId);

		protected abstract void OnRegistered(Context context, string registrationId);

		protected abstract void OnUnRegistered(Context context, string registrationId);

		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				var context = ApplicationContext;
				var action = intent.Action;

				if (action.Equals(Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK))
				{
					handleRegistration(context, intent);
				}
				else if (action.Equals(Constants.INTENT_FROM_GCM_MESSAGE))
				{
					// checks for special messages
					var messageType = intent.GetStringExtra(Constants.EXTRA_SPECIAL_MESSAGE);
					if (messageType != null)
					{
						if (messageType.Equals(Constants.VALUE_DELETED_MESSAGES))
						{
							var sTotal = intent.GetStringExtra(Constants.EXTRA_TOTAL_DELETED);
							if (!string.IsNullOrEmpty(sTotal))
							{
								int nTotal;
								if (int.TryParse(sTotal, out nTotal))
								{
									Logger.Debug("Received deleted messages notification: " + nTotal);
									OnDeletedMessages(context, nTotal);
								}
								else
									Logger.Debug("GCM returned invalid number of deleted messages: " + sTotal);
							}
						}
						else
						{
							// application is not using the latest GCM library
							Logger.Debug("Received unknown special message: " + messageType);
						}
					}
					else
					{
						OnMessage(context, intent);
					}
				}
				else if (action.Equals(Constants.INTENT_FROM_GCM_LIBRARY_RETRY))
				{
					var token = intent.GetStringExtra(EXTRA_TOKEN);

					if (!string.IsNullOrEmpty(token) && !TOKEN.Equals(token))
					{
						// make sure intent was generated by this class, not by a
						// malicious app.
						Logger.Debug("Received invalid token: " + token);
						return;
					}

					// retry last call
					if (GcmClient.IsRegistered(context))
						GcmClient.internalUnRegister(context);
					else
						GcmClient.internalRegister(context, SenderIds);
				}
			}
			finally
			{
				// Release the power lock, so phone can get back to sleep.
				// The lock is reference-counted by default, so multiple
				// messages are ok.

				// If OnMessage() needs to spawn a thread or do something else,
				// it should use its own lock.
				lock (LOCK)
				{
					//Sanity check for null as this is a public method
					if (sWakeLock != null)
					{
						Logger.Debug("Releasing Wakelock");
						sWakeLock.Release();
					}
					else
					{
						//Should never happen during normal workflow
						Logger.Debug("Wakelock reference is null");
					}
				}
			}
		}

		internal static void RunIntentInService(Context context, Intent intent, Type classType)
		{
			lock (LOCK)
			{
				if (sWakeLock == null)
				{
					// This is called from BroadcastReceiver, there is no init.
					var pm = PowerManager.FromContext(context);
					sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, WAKELOCK_KEY);
				}
			}

			Logger.Debug("Acquiring wakelock");
			lock (LOCK)
			{
				sWakeLock.Acquire();
			}
			//intent.SetClassName(context, className);
			intent.SetClass(context, classType);

			context.StartService(intent);
		}

		private void handleRegistration(Context context, Intent intent)
		{
			var registrationId = intent.GetStringExtra(Constants.EXTRA_REGISTRATION_ID);
			var error = intent.GetStringExtra(Constants.EXTRA_ERROR);
			var unregistered = intent.GetStringExtra(Constants.EXTRA_UNREGISTERED);

			Logger.Debug("handleRegistration: registrationId = " + registrationId + ", error = " + error + ", unregistered = " + unregistered);

			// registration succeeded
			if (registrationId != null)
			{
				GcmClient.ResetBackoff(context);
				GcmClient.SetRegistrationId(context, registrationId);
				OnRegistered(context, registrationId);
				return;
			}

			// unregistration succeeded
			if (unregistered != null)
			{
				// Remember we are unregistered
				GcmClient.ResetBackoff(context);
				var oldRegistrationId = GcmClient.ClearRegistrationId(context);
				OnUnRegistered(context, oldRegistrationId);
				return;
			}

			// last operation (registration or unregistration) returned an error;
			Logger.Debug("Registration error: " + error);
			// Registration failed
			if (Constants.ERROR_SERVICE_NOT_AVAILABLE.Equals(error))
			{
				var retry = OnRecoverableError(context, error);

				if (retry)
				{
					int backoffTimeMs = GcmClient.GetBackoff(context);
					int nextAttempt = backoffTimeMs / 2 + sRandom.Next(backoffTimeMs);

					Logger.Debug("Scheduling registration retry, backoff = " + nextAttempt + " (" + backoffTimeMs + ")");

					var retryIntent = new Intent(Constants.INTENT_FROM_GCM_LIBRARY_RETRY);
					retryIntent.PutExtra(EXTRA_TOKEN, TOKEN);

					var retryPendingIntent = PendingIntent.GetBroadcast(context, 0, retryIntent, PendingIntentFlags.OneShot);

					var am = AlarmManager.FromContext(context);
					am.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + nextAttempt, retryPendingIntent);

					// Next retry should wait longer.
					if (backoffTimeMs < MAX_BACKOFF_MS)
					{
						GcmClient.SetBackoff(context, backoffTimeMs * 2);
					}
				}
				else
				{
					Logger.Debug("Not retrying failed operation");
				}
			}
			else
			{
				// Unrecoverable error, notify app
				OnError(context, error);
			}
		}
	}
}