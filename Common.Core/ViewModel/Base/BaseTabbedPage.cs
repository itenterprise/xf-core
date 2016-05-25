using System;
using System.Collections.Generic;
using Common.Core.Helpers;
using Common.Core.Interfaces;
using Common.Core.Tools;
using Common.Core.WebService;
using Xamarin.Forms;

namespace Common.Core.ViewModel
{
	/// <summary>
	/// Базовый класс с закладками
	/// </summary>
	public class BaseTabbedPage<TViewModel> : MainBaseTabbedPage where TViewModel : BaseViewModel, new()
	{
		protected TViewModel _viewModel;

		public TViewModel ViewModel
		{
			get
			{
				return _viewModel ?? (_viewModel = CreateViewModel());
			}
		}

		~BaseTabbedPage()
		{
			_viewModel = null;
		}

		public BaseTabbedPage()
		{
			BindingContext = ViewModel;
		}

		protected virtual TViewModel CreateViewModel()
		{
			return new TViewModel();
		}
	}

	public class MainBaseTabbedPage : TabbedPage
	{
		bool _hasSubscribed;

		public Color BarTextColor
		{
			get;
			set;
		}

		public Color BarBackgroundColor
		{
			get;
			set;
		}

		public MainBaseTabbedPage()
		{
			/*
			Debug.WriteLine("Constructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));
			
			BarBackgroundColor = (Color)ApplicationBase.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;
			BackgroundColor = Color.White;
			*/
			subscribeToIncomingPayload();
		}

		~MainBaseTabbedPage()
		{
			//Debug.WriteLine("Destructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));
		}

		protected virtual void SubscribeToMessages()
		{
		}

		void subscribeToIncomingPayload()
		{
			var weakSelf = new WeakReference(this);
			Action<ApplicationBase, NotificationPayload> action = (app, payload) =>
			{
				var self = (MainBaseTabbedPage)weakSelf.Target;
				self.OnIncomingPayload(payload);
			};
			MessagingCenter.Subscribe(this, Messages.IncomingPayloadReceived, action);
		}

		public bool HasInitialized
		{
			get;
			private set;
		}

		protected virtual void OnLoaded()
		{
		}

		protected virtual void ShowTips(HashSet<string> showedTips)
		{
		}

		protected virtual void Initialize()
		{
		}

		protected override void OnAppearing()
		{
			TrackPage();
			var nav = Parent as NavigationPage;
			if (nav != null)
			{
				nav.BarBackgroundColor = BarBackgroundColor;
				nav.BarTextColor = BarTextColor;
			}

			if (!HasInitialized)
			{
				HasInitialized = true;
				SubscribeToMessages();
				var showedTips = Settings.ShowedTips;
				ShowTips(showedTips);
				Settings.ShowedTips = showedTips;
				OnLoaded();
			}
			ApplicationBase.Current.ProcessPendingPayload();
			base.OnAppearing();
		}

		/// <summary>
		/// Wraps the ContentPage within a NavigationPage
		/// </summary>
		/// <returns>The navigation page.</returns>
		public NavigationPage WithinNavigationPage()
		{
			var nav = new NavigationPage(this);
			//ApplyTheme(nav);
			return nav;
		}
		/*
		protected void SetTheme(League l)
		{
			if (l == null || l.Theme == null)
				return;

			BarBackgroundColor = l.Theme.Light;
			BarTextColor = l.Theme.Dark;
		}
		public void ApplyTheme(NavigationPage nav)
		{
			nav.BarBackgroundColor = BarBackgroundColor;
			nav.BarTextColor = BarTextColor;
		}
		*/

		public void AddDoneButton(string text = null, Page page = null)
		{
			text = text ?? Properties.Resources.Done;
			var btnDone = new ToolbarItem { Text = text };

			btnDone.Clicked += async (sender, e) =>
			await Navigation.PopModalAsync();

			page = page ?? this;
			page.ToolbarItems.Add(btnDone);
		}

		protected virtual void TrackPage()
		{
			ApplicationBase.TrackPage(GetType().Name);
		}

		protected virtual void OnIncomingPayload(NotificationPayload payload)
		{
		}

		#region Authentication

		protected async void LogoutUser()
		{
			var decline = await DisplayAlert(
				Properties.Resources.ApplicationExit,
				Properties.Resources.ExitSure,
				Properties.Resources.Exit,
				Properties.Resources.Cancel);
			if (!decline)
			{
				return;
			}
			LoginHelper.Logout();
			ApplicationBase.Current.StartRegistrationFlow();
		}

		#endregion
	}
}
