using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Core.Tools;
using Common.Core.ViewModel;
using Xamarin.Forms;

namespace Common.Core.Controls
{
	public partial class ContextHelpSearchbar
	{
		#region bindableproperties

		/// <summary>
		/// The suggestion item data template property.
		/// </summary>
		public static readonly BindableProperty SuggestionItemDataTemplateProperty =
			BindableProperty.Create<ContextHelpSearchbar, DataTemplate>(p => p.SuggestionItemDataTemplate, null, BindingMode.TwoWay,
				null, suggestionItemDataTemplateChanged);

		/// <summary>
		/// The placeholder property.
		/// </summary>
		public static readonly BindableProperty PlaceholderProperty =
			BindableProperty.Create<ContextHelpSearchbar, string>(p => p.Placeholder, string.Empty, BindingMode.TwoWay, null,
				placeHolderChanged);

		/// <summary>
		/// The search command property.
		/// </summary>
		public static readonly BindableProperty SearchCommandProperty =
			BindableProperty.Create<ContextHelpSearchbar, ICommand>(p => p.SearchCommand, null, BindingMode.TwoWay, null, searchCommandPropertyChanged);

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

		/// <summary>
		/// Gets or sets the search command.
		/// </summary>
		/// <value>The search command.</value>
		public Command SearchCommand
		{
			get { return (Command)GetValue(SearchCommandProperty); }
			set { SetValue(SearchCommandProperty, value); }
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

		public ContextHelpSearchbar ()
		{
			ContextHelpEnabled = true;
			InitializeComponent();
			SearchBar.TextChanged += (sender, e) => {
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
			if (!string.IsNullOrWhiteSpace(searchString))
			{
				//todo: что это за хрень?
				ViewModel.SearchString = ViewModel.SearchString ?? searchString;
				var words = ViewModel.SearchString.Split(' ', '\r', '\n');
				if (words.Length <= 1)
				{
					ViewModel.SearchString = searchString;
				}
				words[words.Length - 1] = searchString;
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
			var searchString = e.NewTextValue;
			if (!ContextHelpEnabled || string.IsNullOrEmpty(searchString))
			{
				ViewModel.ClearContext();
			}
			else
			{
				ViewModel.ReReadSuggestions(searchString);
			}
			ContextHelpEnabled = true;
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
			var autoCompleteSearch = obj as ContextHelpSearchbar;
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
			var autoCompleteSearch = obj as ContextHelpSearchbar;
			if (autoCompleteSearch != null)
			{
				autoCompleteSearch.SearchBar.Placeholder = newPlaceHolderValue;
			}
		}

		private static void searchCommandPropertyChanged(BindableObject obj, ICommand oldValue, ICommand newValue)
		{
			var autoCompleteSearch = obj as ContextHelpSearchbar;
			if (autoCompleteSearch != null)
			{
				autoCompleteSearch.SearchBar.SearchCommand = newValue;
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

	public abstract class ContextHelpViewModelBase : BaseViewModel
	{
		/// <summary>
		/// Поисковый запрос
		/// </summary>
		public string SearchString
		{
			get { return _searchString; }
			set { SetPropertyChanged(ref _searchString, value); }
		}
		private string _searchString;

		/// <summary>
		/// Контекстная помощь
		/// </summary>
		public ObservableCollectionFast<object> Suggestions
		{
			get { return _suggestions; }
			set
			{
				SetPropertyChanged(ref _suggestions, value);
			}
		}
		private ObservableCollectionFast<object> _suggestions = new ObservableCollectionFast<object>();

		public bool IsSuggestionsVisible
		{
			get { return _isSuggestionsVisible; }
			set
			{
				SetPropertyChanged(ref _isSuggestionsVisible, value);
				OnSuggestionsVisibleChanged();
			}
		}
		private bool _isSuggestionsVisible;

		public Command SearchCommand { get; set; }

		protected ContextHelpViewModelBase()
		{
			Suggestions.CollectionChanged += (sender, args) => {
				IsSuggestionsVisible = Suggestions.Count > 0;
			};
		}

		public abstract string GetValue(object item);

		public event EventHandler SuggestionsVisibleChanged;

		public abstract Task<List<object>> GetSuggestions(string messageText);

		public void ClearContext()
		{
			Suggestions.Clear();
		}

		public async Task ReReadSuggestions(string searchString)
		{
			var suggestions = await GetSuggestions(searchString);
			if (suggestions == null)
			{
				ClearContext();
				return;
			}
			Suggestions.Reset(suggestions);
		}

		protected virtual void OnSuggestionsVisibleChanged()
		{
			var handler = SuggestionsVisibleChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}