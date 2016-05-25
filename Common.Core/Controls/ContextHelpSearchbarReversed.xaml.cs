using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Core.Tools;
using Common.Core.ViewModel;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public partial class ContextHelpSearchbarReversed
	{
		#region bindableproperties

		/// <summary>
		/// The suggestion item data template property.
		/// </summary>
		public static readonly BindableProperty SuggestionItemDataTemplateProperty =
			BindableProperty.Create<ContextHelpSearchbarReversed, DataTemplate>(p => p.SuggestionItemDataTemplate, null, BindingMode.TwoWay,
				null, suggestionItemDataTemplateChanged);

		/// <summary>
		/// The placeholder property.
		/// </summary>
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create<ContextHelpSearchbarReversed, string>(p => p.Placeholder, string.Empty, BindingMode.TwoWay, null,
				placeHolderChanged);

		#endregion

		/// <summary>
		/// Gets or sets the suggestion item data template.
		/// </summary>
		/// <value>The sugestion item data template.</value>
		public DataTemplate SuggestionItemDataTemplate
		{
			get { return (DataTemplate)GetValue(SuggestionItemDataTemplateProperty); }
			set { SetValue(SuggestionItemDataTemplateProperty, value); }
		}

		/// <summary>
		/// Gets or sets the placeholder.
		/// </summary>
		/// <value>The placeholder.</value>
		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}

		public ExpandableEditor TextEntry
		{
			get { return SearchBar; }
		}

		public bool ContextHelpEnabled { get; set; }

		public ContextHelpViewModelBase ViewModel
		{
			get { return _viewModel; }
			set
			{
				_viewModel = value;
				BindingContext = value;
			}
		}

		private ContextHelpViewModelBase _viewModel;

		public event EventHandler<object> SuggestionSelected;

		public event EventHandler<TextChangedEventArgs> TextChanged;

		public ContextHelpSearchbarReversed()
		{
			ContextHelpEnabled = true;
			InitializeComponent();
			SearchBar.TextChanged += (sender, e) =>
			{
				searchTextChanged(sender, e);
				if (TextChanged != null)
				{
					TextChanged(sender, e);
				}
			};
		}

		public void Search(string searchString = null)
		{
			ContextHelpEnabled = false;


			ViewModel.SearchString = searchString;
			if (!string.IsNullOrWhiteSpace(searchString))
			{
				//todo: что это за хрень?
				ViewModel.SearchString = ViewModel.SearchString ?? searchString;
				var words = ViewModel.SearchString.Split(' ', '\r', '\n');
				if (words.Length <= 1)
				{
					ViewModel.SearchString = searchString;
				}
				ViewModel.SearchString = string.Join(" ", words);
			}
			var searchCommand = ViewModel.SearchCommand;
			if (searchCommand == null)
			{
				return;
			}
			searchCommand.ChangeCanExecute();
			searchCommand.Execute(null);
			searchCommand.ChangeCanExecute();
		}

		private void searchTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!ContextHelpEnabled || e.NewTextValue.Equals(e.OldTextValue))
			{
				if (ViewModel != null)
				{
					ViewModel.ClearContext();
					SuggestionsList.HeightRequest = 0;
				}
			}
			else
			{
				reReadSuggestions(e.NewTextValue);
			}
		}

		private async void reReadSuggestions(string searchString)
		{
			await ViewModel.ReReadSuggestions(searchString);

			SuggestionsList.HeightRequest = ViewModel.Suggestions.Count <= 3 ? ViewModel.Suggestions.Count * 50 : 150;
		}

		#region propertychanged events

		/// <summary>
		/// Suggestions the item data template changed.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="oldShowSearchValue">The old show search value.</param>
		/// <param name="newShowSearchValue">The new show search value.</param>
		private static void suggestionItemDataTemplateChanged(BindableObject obj, DataTemplate oldShowSearchValue,
			DataTemplate newShowSearchValue)
		{
			var autoCompleteSearch = obj as ContextHelpSearchbarReversed;
			if (autoCompleteSearch != null)
			{
				autoCompleteSearch.SuggestionsList.ItemTemplate = newShowSearchValue;
			}
		}

		/// <summary>
		/// Places the holder changed.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="oldPlaceHolderValue">The old place holder value.</param>
		/// <param name="newPlaceHolderValue">The new place holder value.</param>
		private static void placeHolderChanged(BindableObject obj, string oldPlaceHolderValue, string newPlaceHolderValue)
		{
			var autoCompleteSearch = obj as ContextHelpSearchbarReversed;
			if (autoCompleteSearch != null)
			{
				autoCompleteSearch.SearchBar.Placeholder = newPlaceHolderValue;
			}
		}

		#endregion

		private void searchButtonPressed(object sender, EventArgs e)
		{
			ViewModel.ClearContext();
		}

		private void suggestionSelected(object sender, SelectedItemChangedEventArgs e)
		{
			OnSuggestionSelected(e.SelectedItem);
			SuggestionsList.SelectedItem = null;
		}

		protected virtual void OnSuggestionSelected(object e)
		{
			var handler = SuggestionSelected;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}