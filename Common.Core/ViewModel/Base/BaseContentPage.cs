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
	/// Базовый класс страницы
	/// </summary>
	public class BaseContentPage<TViewModel> : MainBaseContentPage where TViewModel : BaseViewModel, new()
	{
		protected TViewModel _viewModel;

		public TViewModel ViewModel
		{
			get
			{
				return _viewModel ?? ( _viewModel = CreateViewModel());
			}
		}

		protected virtual TViewModel CreateViewModel()
		{
			return new TViewModel();
		}

		~BaseContentPage()
		{
			_viewModel = null;
		}

		public BaseContentPage()
		{
			BindingContext = ViewModel;
		}

		public BaseContentPage(TViewModel viewModel)
		{
			_viewModel = viewModel;
			BindingContext = ViewModel;
		}
	}

	public class MainBaseContentPage : ContentPage
	{
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

		public MainBaseContentPage()
		{
			/*
			Debug.WriteLine("Constructor called for {0} {1}".Fmt(GetType().Name, GetHashCode()));

			BarBackgroundColor = (Color)ApplicationBase.Current.Resources["grayPrimary"];
			BarTextColor = Color.White;
			BackgroundColor = Color.White;
			*/
			subscribeToIncomingPayload();
		}

		protected virtual Dictionary<string, string> GetTrackMetadata()
		{
			return new Dictionary<string, string>();
		}

		protected virtual void SubscribeToMessages()
		{
		}

		private void subscribeToIncomingPayload()
		{
			var weakSelf = new WeakReference(this);
			Action<ApplicationBase, NotificationPayload> action = (app, payload) =>
			{
				var self = (MainBaseContentPage)weakSelf.Target;
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

		protected override void OnAppearing()
		{
			base.OnAppearing();
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

		public void AddCancelButton(string text = null, Page page = null, EventHandler click = null)
		{
			text = text ?? Properties.Resources.Cancel;
			var cancelButton = new ToolbarItem
			{
				Text = text,
			};
			if (Device.OS == TargetPlatform.iOS)
			{
				cancelButton.Priority = -2;
			}

			cancelButton.Clicked += async (sender, e) => await Navigation.PopModalAsync();
			if (click != null)
			{
				cancelButton.Clicked += click;
			}

			page = page ?? this;
			page.ToolbarItems.Add(cancelButton);
		}

		public void AddDoneButton(string text = null, Page page = null, EventHandler click = null)
		{
			text = text ?? Properties.Resources.Done;
			var btnDone = new ToolbarItem
			{
				Text = text
			};
			if (Device.OS == TargetPlatform.iOS)
			{
				btnDone.Priority = -1;
			}

			btnDone.Clicked += async (sender, e) => await Navigation.PopModalAsync();
			if (click != null)
			{
				btnDone.Clicked += click;
			}

			page = page ?? this;
			page.ToolbarItems.Add(btnDone);
		}

		protected virtual void TrackPage()
		{
			ApplicationBase.TrackPage(GetType().Name, GetTrackMetadata());
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
			await ApplicationBase.Current.MainPage.Navigation.PopModalAsync();
			ApplicationBase.Current.StartRegistrationFlow();
		}

		#endregion
	}
}