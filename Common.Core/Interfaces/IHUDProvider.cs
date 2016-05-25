using System;
using System.Threading.Tasks;
using Common.Core.Helpers;

namespace Common.Core.Interfaces
{
	public interface IHUDProvider
	{
		void DisplayProgress(string message);

		void DisplaySuccess(string message);

		void DisplayError(string message);

		void Dismiss();
	}

	public class HUD : IDisposable
	{
		bool _cancel;

		public HUD(string message)
		{
			StartHUD(message);
		}

		async void StartHUD(string message)
		{
			await Task.Delay(100);

			if (_cancel)
			{
				_cancel = false;
				return;
			}

			_cancel = false;
			Settings.Hud.DisplayProgress(message);
		}

		public void Dispose()
		{
			_cancel = true;
			Settings.Hud.Dismiss();
		}
	}
}
