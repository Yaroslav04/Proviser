using Proviser.Models;
using Proviser.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser.ViewModels
{
    internal class CourtsHearingViewModel : BaseViewModel
    {
        public CourtsHearingViewModel()
        {
            Title = "Заседания " + DateTime.Now.ToShortDateString();
            Items = new ObservableCollection<Courts>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Courts>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        #region Properties

        private Courts _selectedItem;

        public Courts SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }
        public ObservableCollection<Courts> Items { get; }

        #endregion

        #region Commands

        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Courts> ItemTapped { get; }

        #endregion

        #region Functions
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
                foreach (var item in items)
                {
                    var _case = await App.DataBase.GetCasesByCaseAsync(item.Case);
                    item.SoketHeader = _case.Header;
                    item.SoketNote = _case.Note;
                    if (_case.PrisonDate > System.Convert.ToDateTime("01.01.2000 00:00:00"))
                    {
                        item.SoketPrisonDate = $"Дата содержания под стражей: {_case.PrisonDate.ToShortDateString()}";
                    }               
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(CaseAdd));
        }

        async void OnItemSelected(Courts item)
        {

            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(Case)}?{nameof(CaseViewModel.Id)}={item.Case}");
        }

        #endregion
    }
}
