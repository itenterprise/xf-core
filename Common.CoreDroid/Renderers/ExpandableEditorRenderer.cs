using System.ComponentModel;
using Android.App;
using Common.Core.Controls;
using Common.CoreDroid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views.InputMethods;
using Plugin.CurrentActivity;

[assembly: ExportRenderer(typeof(ExpandableEditor), typeof(ExpandableEditorRenderer))]
namespace Common.CoreDroid.Renderers
{
	public class ExpandableEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(
			ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				var element = e.NewElement as ExpandableEditor;
				this.Control.Hint = element.Placeholder;
			}
		}

		protected override void OnElementPropertyChanged(
			object sender,
			PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			var editor = Element as ExpandableEditor;
			if (Control == null || editor == null)
			{
				return;
			}
			if (e.PropertyName == ExpandableEditor.PlaceholderProperty.PropertyName)
			{
				this.Control.Hint = editor.Placeholder;
			}
			//else if (e.PropertyName == ExpandableEditor.IsVisibleKeyBoardProperty.PropertyName)
			//{
			//	if (!editor.IsVisibleKeyBoard)
			//	{
			//		InputMethodManager imm = (InputMethodManager)CrossCurrentActivity.Current.Activity.GetSystemService("input_method");
			//		imm.HideSoftInputFromWindow(Control.WindowToken, 0);
			//	}
			//}
		}
	}
}