using Xamarin.Forms;

namespace Common.Core.Controls
{
	public class AnimatedButton : Button
	{
		public AnimatedButton()
		{
			const int animationTime = 100;
			Clicked += async (sender, e) =>
			{
				var btn = (AnimatedButton)sender;
				await btn.ScaleTo(1.2, animationTime);
				await btn.ScaleTo(1, animationTime);
			};
		}
	}
}