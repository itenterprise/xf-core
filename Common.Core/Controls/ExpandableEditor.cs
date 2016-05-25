using System;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class ExpandableEditor : Editor
	{
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create<ExpandableEditor, string>(view => view.Placeholder, string.Empty);

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public static readonly BindableProperty IsVisibleKeyBoardProperty =
			BindableProperty.Create<ExpandableEditor, bool>(view => view.IsVisibleKeyBoard, true);

		public bool IsVisibleKeyBoard
		{
			get { return (bool)GetValue(IsVisibleKeyBoardProperty); }
			set { SetValue(IsVisibleKeyBoardProperty, value); }
		}

		public ExpandableEditor()
		{
			TextChanged += onTextChanged;
		}

		public void EndEdit()
		{
			Device.BeginInvokeOnMainThread(() => {
				IsVisibleKeyBoard = false;
				if (Device.OS != TargetPlatform.iOS)
				{
					Unfocus();
				}
			});
		}

		private void onTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			InvalidateMeasure();
		}
	}
}