using Proviser.Models;
using Proviser.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Proviser.ViewModels
{
    internal class CasesViewModel : BaseViewModel
    {
        public CasesViewModel()
        {
            Title = "Судебные дела";

            Items = new ObservableCollection<Models.Cases>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Models.Cases>(OnItemSelected);
      

            AddItemCommand = new Command(OnAddItem);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        #region Properties

        private Models.Cases _selectedItem;
        public ObservableCollection<Models.Cases> Items { get; }


        #endregion

        #region Commands

        public Command<Models.Cases> ItemTapped { get; }   
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }

        #endregion

        #region Functions

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();


                var items = await App.DataBase.GetCasesAsync();
                items = items.OrderBy(x => x.Header).ToList();
                foreach (var item in items)
                {
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

        public Models.Cases SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(CaseAdd));
        }

        async void OnItemSelected(Models.Cases item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(Case)}?{nameof(CaseViewModel.Id)}={item.Case}");
        }

        #endregion
    }
}
