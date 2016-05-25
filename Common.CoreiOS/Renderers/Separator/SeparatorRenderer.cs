﻿using System.ComponentModel;
using Common.Core.Controls;
using Common.CoreiOS.Renderers.Separator;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Separator), typeof(SeparatorRenderer))]
namespace Common.CoreiOS.Renderers.Separator
{
	/// <summary>
	/// Class SeparatorRenderer.
	/// </summary>
	public class SeparatorRenderer : ViewRenderer<Core.Controls.Separator, UISeparator>
	{
		/// <summary>
		/// Called when [element changed].
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<Core.Controls.Separator> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				if (Control == null)
				{
					BackgroundColor = Color.Transparent.ToUIColor();
					SetNativeControl(new UISeparator(Bounds));
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
			separator.Thickness = Element.Thickness;
			separator.StrokeColor = Element.Color.ToUIColor();
			separator.StrokeType = Element.StrokeType;
			separator.Orientation = Element.Orientation;
			separator.SpacingBefore = Element.SpacingBefore;
			separator.SpacingAfter = Element.SpacingAfter;
		}
	}
}

