namespace Common.Core.ViewModel
{
	public class ErrorViewModel : BaseViewModel
	{
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { SetPropertyChanged(ref _errorMessage, value); }
		}
		private string _errorMessage;
	}
}