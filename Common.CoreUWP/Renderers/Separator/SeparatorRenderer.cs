using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Common.Core.Controls;
using Common.CoreUWP.Providers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace Common.CoreUWP.Providers
{
	/// <summary>
	/// Class SeparatorRenderer.
	/// </summary>
	public class SeparatorRenderer : ViewRenderer<Separator, AppBarSeparator>
	{
		/// <summary>
		/// Called when [element changed].
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<Separator> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					SetNativeControl(new AppBarSeparator());
				}
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
			if (Control == null || Element == null)
				return;

			var separator = Control;
			//todo:
		}
	}
}

