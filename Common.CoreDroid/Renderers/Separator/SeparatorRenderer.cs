using System.ComponentModel;
using Common.Core.Controls;
using Common.CoreDroid.Renderers.Separator;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace Common.CoreDroid.Renderers.Separator
{
	class SeparatorRenderer : ViewRenderer<Core.Controls.Separator, SeparatorView>
	{
		/// <summary>
		/// Called when [element changed].
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<Core.Controls.Separator> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement == null)
			{
				return;
			}

			if (Control == null)
			{
				SetNativeControl(new SeparatorView(Context));
			}

			setProperties();
		}


		/// <summary>
		/// Handles the <see cref="E:ElementPropertyChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			setProperties();
		}

		/// <summary>
		/// Sets the properties.
		/// </summary>
		private void setProperties()
		{
			Control.SpacingBefore = Element.SpacingBefore;
			Control.SpacingAfter = Element.SpacingAfter;
			Control.Thickness = Element.Thickness;
			Control.StrokeColor = Element.Color.ToAndroid();
			Control.StrokeType = Element.StrokeType;
			Control.Orientation = Element.Orientation;

			Control.Invalidate();
		}
	}
}