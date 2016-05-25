using System.Linq;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class AutoAdjustingEditor : Editor
	{
		private bool _sized;
		public double LineHeight { get; set; }

		public string PlaceHolder
		{
			get { return _placeHolder; }
			set
			{
				_placeHolder = value;
				if (string.IsNullOrEmpty(Text))
				{
					Text = PlaceHolder;
				}
			}
		}
		private string _placeHolder;

		public AutoAdjustingEditor()
		{
			LineHeight = 0d;
			TextChanged += (sender, arguments) => {
				if (LineHeight == 0)
				{
					return;
				}
				var count = (string.IsNullOrEmpty(Text)
					? 0
					: Text.ToCharArray().Count(c => c == '\n')) + 1;
				var newHeight = count == 0
					? LineHeight
					: (LineHeight) * count;
				newHeight *= (double)count / (2 * count - 1);
				if (Height != newHeight)
				{
					HeightRequest = newHeight;
				}
			};

			setUpPlaceHolder();
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			if (!_sized)
			{
				var count = (string.IsNullOrEmpty(Text) 
					? 0 
					: Text.ToCharArray().Count(c => c == '\n')) + 1;
				LineHeight = height / count;
				_sized = true;
			}
			base.OnSizeAllocated(width, height);
		}

		private void setUpPlaceHolder()
		{
			PlaceHolder = string.Empty;

			Unfocused += (sender, arguments) =>
			{
				if (string.IsNullOrEmpty(Text))
				{
					Text = PlaceHolder;
				}
			};
			Focused += (sender, arguments) =>
			{
				if (Tools.Text.CompareEx(Text, PlaceHolder))
				{
					Text = string.Empty;
				}
			};
		}
	}
}