using System.ComponentModel;
using Common.Core.Controls;
using Common.CoreiOS.Renderers;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace Common.CoreiOS.Renderers
{
	public class ExpandableEditorRenderer : EditorRenderer
	{
		private string Placeholder { get; set; }

		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (Control != null)
			{
				Control.ScrollEnabled = false;
				Control.Layer.BorderWidth = 1f;
				Control.Layer.CornerRadius = 5f;
				Control.ClipsToBounds = true;
				Control.Layer.BorderColor = new CGColor(UIColor.Gray.CGColor, 0.2f);
				Control.InputAccessoryView = null;
				var insets = Control.TextContainerInset;
				Control.TextContainerInset = new UIEdgeInsets(insets.Top / 2, insets.Left, insets.Bottom / 2, insets.Right);
				Control.LayoutMargins = UIEdgeInsets.Zero;
				var element = Element as ExpandableEditor;
				if (element != null)
				{
					Placeholder = element.Placeholder;
					Control.TextColor = UIColor.LightGray;
					Control.Text = Placeholder;

					Control.ShouldBeginEditing += textView => {
						if (textView.Text == Placeholder)
						{
							textView.Text = string.Empty;
						}
						textView.TextColor = UIColor.Black; // Text Color
						return true;
					};

					Control.ShouldEndEditing += textView => {
						if (string.IsNullOrEmpty(textView.Text))
						{
							textView.Text = Placeholder;
							textView.TextColor = UIColor.LightGray; // Placeholder Color
						}
						return true;
					};
				}
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			var editor = Element as ExpandableEditor;
			if (Control == null || editor == null)
			{
				return;
			}
			if (e.PropertyName == ExpandableEditor.IsVisibleKeyBoardProperty.PropertyName)
			{
				Control.EndEditing(true);
			}
		}
	}
}