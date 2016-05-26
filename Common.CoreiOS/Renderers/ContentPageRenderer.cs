﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.CoreiOS.Renderers;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
namespace Common.CoreiOS.Renderers
{
	public class ContentPageRenderer : KeyboardOverlapRenderer
	{
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (NavigationController == null)
			{
				return;
			}
			var contentPage = Element as ContentPage;
			if (contentPage == null)
			{
				return;
			}
			var itemsInfo = contentPage.ToolbarItems;

			var navigationItem = NavigationController.TopViewController.NavigationItem;
			var leftNativeButtons = (navigationItem.LeftBarButtonItems ?? new UIBarButtonItem[] { }).ToList();
			var rightNativeButtons = (navigationItem.RightBarButtonItems ?? new UIBarButtonItem[] { }).ToList();

			rightNativeButtons.ForEach(nativeItem => {
				var info = GetButtonInfo(itemsInfo, nativeItem.Title);

				if (info == null)
				{
					return;
				}
				if (info.Priority == -2)
				{
					leftNativeButtons.Add(nativeItem);
				}
				if (info.Priority == -1)
				{
					nativeItem.Style = UIBarButtonItemStyle.Done;
				}
			});
			foreach (var leftNativeButton in leftNativeButtons)
			{
				rightNativeButtons.Remove(leftNativeButton);
			}
			navigationItem.RightBarButtonItems = rightNativeButtons.ToArray();
			navigationItem.LeftBarButtonItems = leftNativeButtons.ToArray();
		}

		private ToolbarItem GetButtonInfo(IList<ToolbarItem> items, string name)
		{
			if (string.IsNullOrEmpty(name) || items == null)
			{
				return null;
			}
			return items.ToList().FirstOrDefault(itemData => name.Equals(itemData.Name));
		}
	}

	public class KeyboardOverlapRenderer : PageRenderer
	{
		NSObject _keyboardShowObserver;
		NSObject _keyboardHideObserver;
		private bool _pageWasShiftedUp;
		private double _activeViewBottom;
		private bool _isKeyboardShown;

		public static void Init()
		{
			var now = DateTime.Now;
			Debug.WriteLine("Keyboard Overlap plugin initialized {0}", now);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			var page = Element as ContentPage;

			if (page != null)
			{
				var contentScrollView = page.Content as ScrollView;

				if (contentScrollView != null)
					return;

				RegisterForKeyboardNotifications();
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			UnregisterForKeyboardNotifications();
		}

		void RegisterForKeyboardNotifications()
		{
			if (_keyboardShowObserver == null)
				_keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardShow);
			if (_keyboardHideObserver == null)
				_keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardHide);
		}

		void UnregisterForKeyboardNotifications()
		{
			_isKeyboardShown = false;
			if (_keyboardShowObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
				_keyboardShowObserver.Dispose();
				_keyboardShowObserver = null;
			}

			if (_keyboardHideObserver != null)
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
				_keyboardHideObserver.Dispose();
				_keyboardHideObserver = null;
			}
		}

		protected virtual void OnKeyboardShow(NSNotification notification)
		{
			if (!IsViewLoaded || _isKeyboardShown)
				return;

			_isKeyboardShown = true;
			var activeView = View.FindFirstResponder();

			if (activeView == null)
				return;

			var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
			var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

			if (!isOverlapping)
				return;

			if (isOverlapping)
			{
				_activeViewBottom = activeView.GetViewRelativeBottom(View) + 5;
				ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
			}
		}

		private void OnKeyboardHide(NSNotification notification)
		{
			if (!IsViewLoaded)
				return;

			_isKeyboardShown = false;
			var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);

			if (_pageWasShiftedUp)
			{
				ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
			}
		}

		private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
		{
			var pageFrame = Element.Bounds;

			var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

			Element.LayoutTo(new Rectangle(pageFrame.X, newY,
				pageFrame.Width, pageFrame.Height));

			_pageWasShiftedUp = true;
		}

		private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
		{
			var pageFrame = Element.Bounds;

			var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

			Element.LayoutTo(new Rectangle(pageFrame.X, newY,
				pageFrame.Width, pageFrame.Height));

			_pageWasShiftedUp = false;
		}

		private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
		{
			return (pageHeight - activeViewBottom) - keyboardHeight;
		}
	}

	public static class ViewExtensions
	{
		/// <summary>
		/// Find the first responder in the <paramref name="view"/>'s subview hierarchy
		/// </summary>
		/// <param name="view">
		/// A <see cref="UIView"/>
		/// </param>
		/// <returns>
		/// A <see cref="UIView"/> that is the first responder or null if there is no first responder
		/// </returns>
		public static UIView FindFirstResponder(this UIView view)
		{
			if (view.IsFirstResponder)
			{
				return view;
			}
			foreach (UIView subView in view.Subviews)
			{
				var firstResponder = subView.FindFirstResponder();
				if (firstResponder != null)
					return firstResponder;
			}
			return null;
		}

		/// <summary>
		/// Returns the new view Bottom (Y + Height) coordinates relative to the rootView
		/// </summary>
		/// <returns>The view relative bottom.</returns>
		/// <param name="view">View.</param>
		/// <param name="rootView">Root view.</param>
		public static double GetViewRelativeBottom(this UIView view, UIView rootView)
		{
			var viewRelativeCoordinates = rootView.ConvertPointFromView(view.Frame.Location, view);
			var activeViewRoundedY = Math.Round(viewRelativeCoordinates.Y, 2);

			return activeViewRoundedY + view.Frame.Height;
		}

		/// <summary>
		/// Determines if the UIView is overlapped by the keyboard
		/// </summary>
		/// <returns><c>true</c> if is keyboard overlapping the specified activeView rootView keyboardFrame; otherwise, <c>false</c>.</returns>
		/// <param name="activeView">Active view.</param>
		/// <param name="rootView">Root view.</param>
		/// <param name="keyboardFrame">Keyboard frame.</param>
		public static bool IsKeyboardOverlapping(this UIView activeView, UIView rootView, CGRect keyboardFrame)
		{
			var activeViewBottom = activeView.GetViewRelativeBottom(rootView);
			var pageHeight = rootView.Frame.Height;
			var keyboardHeight = keyboardFrame.Height;

			var isOverlapping = activeViewBottom >= (pageHeight - keyboardHeight);

			return isOverlapping;
		}
	}

}