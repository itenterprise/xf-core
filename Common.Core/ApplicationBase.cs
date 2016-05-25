using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Acr.UserDialogs;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.Tools;
using Common.Core.View;
using Common.Core.ViewModel;
using Common.Core.WebService;
using FrazzApps.Xamarin.AnalyticsConnector;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Common.Core
{
	public abstract class ApplicationBase : Application
	{
		public static int AnimationSpeed = 250;

		public abstract string NotificationSenderId { get; }

		public abstract string AppId { get; }

		public abstract bool PushNotificationsEnabled { get; }

		public virtual string DefaultUrl
		{
			//todo: переделать на demo-проект
			get { return "https://m.it.ua/ws/webservice.asmx"; }
		}

		public virtual bool AllowToChangeUrl
		{
			get { return true; }
		}

		public new static ApplicationBase Current
		{
			get { return (ApplicationBase)Application.Current; }
		}

		public static NotificationPayload PendingNotificationPayload
		{
			get;
			private set;
		}

		public static bool IsNetworkRechable
		{
			get;
			set;
		}

		public virtual Page AuthenicationPage
		{
			get { return new AuthenticationPage(); }
		}

		public virtual Page LoginPage
		{
			get
			{
				return new LoginPage(new AuthenticationViewModel()) {
					Title = Core.Properties.Resources.Enter,
					Icon = Device.OnPlatform("profile_menu.png", "", "")
				};
			}
		}

		protected ApplicationBase()
		{
			Initialize();
			StartFlow();
		}

		protected virtual void Initialize()
		{
			//индикация подключения к интернету
			IsNetworkRechable = CrossConnectivity.Current.IsConnected;
			CrossConnectivity.Current.ConnectivityChanged += (sender, args) =>
			{
				IsNetworkRechable = args.IsConnected;
			};
			// если истек срок действия тикета - запросить новый
			// если не удалось, вызвать экран входа
			WebServiceHelper.OnTicketExpired += ticketExpired;
			MessagingCenter.Subscribe<BaseViewModel, Exception>(this, "ExceptionOccurred", onAppExceptionOccurred);
			// если в приложении доступны push-уведомления, то подписаться на регистрацию устройства после входа в приложения
			if (PushNotificationsEnabled)
			{
				MessagingCenter.Subscribe<ApplicationBase>(this, Messages.AuthenticationComplete, @base => {
					if (Settings.SignalAboutNotifications)
					{
						SubscribePushNotifications(Settings.Sound);
					}
				});
				// The root page of your application
				MessagingCenter.Subscribe<ApplicationBase>(this, Messages.UserLogout, @base => {
					if (Settings.SignalAboutNotifications)
					{
						UnsubscribePushNotifications();
					}
				});
			}
			// Локализация
			Core.Properties.Resources.Culture = new CultureInfo(Settings.Culture);
		}

		public virtual void SubscribePushNotifications(bool silentMode = false)
		{
			var push = DependencyService.Get<IPushNotifications>();
			push.SubscribePushNotifications(silentMode);
		}

		public virtual void UnsubscribePushNotifications()
		{
			var push = DependencyService.Get<IPushNotifications>();
			push.UnSubscribePushNotifications();
		}

		public virtual void StartFlow()
		{
			StartRegistrationFlow();
			if (Settings.IsAutenticated)
			{
				GotoMainPage();
			}
		}

		public virtual void StartRegistrationFlow()
		{
			MainPage = DependencyService.Get<WelcomeStartPage>();
		}

		public abstract void GotoMainPage();

		public async virtual void GotoAuthenticationPage()
		{
			await MainPage.Navigation.PushModalAsync(new AuthenticationPage(), true);
		}

		protected override void OnStart()
		{
			MessagingCenter.Subscribe<ApplicationBase, NotificationPayload>(this, Messages.IncomingPayloadReceivedInternal, (sender, payload) => OnIncomingPayload(payload));
			base.OnStart();
		}

		protected override void OnSleep()
		{
			//MessagingCenter.Unsubscribe<ApplicationBase, NotificationPayload>(this, Messages.IncomingPayloadReceivedInternal);
			base.OnSleep();
		}

		/// <summary>
		/// This method is here purely to handle shelved push notifications
		/// </summary>
		/// <param name="payload">Payload.</param>
		public void OnIncomingPayload(NotificationPayload payload)
		{
			if (payload == null)
			{
				return;
			}
			if (!Settings.IsAutenticated)
			{
				PendingNotificationPayload = payload;
			}
			else
			{
				MessagingCenter.Send(Current, Messages.IncomingPayloadReceived, payload);
			}
		}

		internal void ProcessPendingPayload()
		{
			if (PendingNotificationPayload == null || !Settings.IsAutenticated)
			{
				return;
			}
			MessagingCenter.Send(Current, Messages.IncomingPayloadReceived, PendingNotificationPayload);
			PendingNotificationPayload = null;
		}

		private async void ticketExpired(WebServiceHelper.TicketExpiredEventArgs args)
		{
			var regetTokenResult =  AsyncHelpers.RunSync(LoginHelper.TryLoginFromCurrentSessionOrSecureStorage);
			if (regetTokenResult != null)
			{
				args.NewTokenTaken = regetTokenResult.Success;
			}
			else
			{
				await MainPage.Navigation.PushModalAsync(AuthenicationPage);
			}
		}

		public static GoogleAnalytics GoogleAnalytics;

		public static void TrackException(Exception e)
		{
#if DEBUG
#else
			if (!IsNetworkRechable)
			{
				return;
			}
			if (GoogleAnalytics == null)
			{
				return;
			}
			try
			{
				GoogleAnalytics.TrackException(Settings.GoogleAnalyticsUserId, e, true);
			}
			catch (Exception)
			{
			}
#endif
		}

		public static void TrackPage(string pageName, Dictionary<string, string> query = null)
		{
#if DEBUG
#else
			if (!IsNetworkRechable)
			{
				return;
			}
			string queryStr = null;
			if (query != null && query.Count > 0)
			{
				var sb = new StringBuilder("/?");
				foreach (var s in query)
				{
					sb.AppendFormat("{0}={1}", s.Key, s.Value);
				}
				queryStr = sb.ToString();
			}
			if (GoogleAnalytics == null)
			{
				return;
			}
			try
			{
				GoogleAnalytics.TrackScreen(Settings.GoogleAnalyticsUserId, string.Concat(pageName, queryStr));
			}
			catch (Exception)
			{
			}
#endif
		}

		/// <summary>
		/// All application exceptions should be routed through this method so they get process/displayed to the user in a consistent manner
		/// </summary>
		private static void onAppExceptionOccurred(BaseViewModel viewModel, Exception exception)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				try
				{
					if (Settings.Hud != null)
					{
						Settings.Hud.Dismiss();
					}

					var msg = exception.Message;
					/*
					var mse = exception as MobileServiceInvalidOperationException;

					if (mse != null)
					{
						var body = await mse.Response.Content.ReadAsStringAsync();
						var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

						if (dict != null && dict.ContainsKey("message"))
							msg = dict["message"].ToString();
					}
					*/
					if (msg.Length > 300)
						msg = msg.Substring(0, 300);

					msg.ToToast(ToastEvent.Error);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e);
				}
			});
		}
	}
}