using Proviser.Models;
using Proviser.Services;
using Proviser.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;
using Proviser.Servises;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Proviser.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class CaseViewModel : BaseViewModel
    {
        public CaseViewModel()
        {
            ItemsHearing = new ObservableCollection<Courts>();
            ItemsDecision = new ObservableCollection<Decision>();
            ItemDecisionTapped = new Command<Decision>(OnItemDecisonsSelected);
            AddDateCommand = new Command(OnAddDateItem);
            AddDatePrisonCommand = new Command(OnAddDatePrisonItem);
            DeleteCommand = new Command(DeleteItem);
            DeleteHearingCommand = new Command(DeleteHearingItem);
            EditNoteCommand = new Command(EditNote);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedDecisionItem = null;
        }

        #region Properties

        int courtHearingSize;
        public int CourtHearingSize
        {
            get
            {
                return courtHearingSize;
            }
            set
            {
                SetProperty(ref courtHearingSize, value);
            }
        }

        int courtDecisionSize;
        public int CourtDecisionSize
        {
            get
            {
                return courtDecisionSize;
            }
            set
            {
                SetProperty(ref courtDecisionSize, value);
            }
        }

        private Decision _selectedDecisionItem;

        public Decision SelectedDecisionItem
        {
            get => _selectedDecisionItem;
            set
            {
                SetProperty(ref _selectedDecisionItem, value);
                OnItemDecisonsSelected(value);
            }
        }

        string id;
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                LoadItemId(value);
                ExecuteLoadHearingCommand(value);
                ExecuteLoadDecisionsCommand(value);
            }
        }

        public ObservableCollection<Courts> ItemsHearing { get; }
        public ObservableCollection<Decision> ItemsDecision { get; }

        private string caseId;
        private string judge;
        private string court;
        private string littigans;
        private string category;
        private string header;
        private string note;
        private string prisonDate;
        public string Case
        {
            get => caseId;
            set => SetProperty(ref caseId, value);
        }
        public string Judge
        {
            get => judge;
            set => SetProperty(ref judge, value);
        }
        public string Court
        {
            get => court;
            set => SetProperty(ref court, value);
        }
        public string Littigans
        {
            get => littigans;
            set => SetProperty(ref littigans, value);
        }
        public string Category
        {
            get => category;
            set => SetProperty(ref category, value);
        }
        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }
        public string Note
        {
            get => note;
            set => SetProperty(ref note, value);
        }

        public string PrisonDate
        {
            get => prisonDate;
            set => SetProperty(ref prisonDate, value);
        }

        #endregion

        #region Commands

        public Command<Decision> ItemDecisionTapped { get; }

        public Command ItemMenuTapped { get; }
        public Command LoadItemsCommand { get; }
        public Command AddDateCommand { get; }
        public Command AddDatePrisonCommand { get; }
        public Command DeleteCommand { get; }
        public Command DeleteHearingCommand { get; }
        public Command EditNoteCommand { get; }

        #endregion

        #region Functions
        public async void LoadItemId(string _id)
        {
            Debug.WriteLine(_id);
            try
            {
                var item = await App.DataBase.GetCasesByCaseAsync(_id);
                Case = item.Case;
                Judge = item.Judge;
                Court = item.Court;
                Littigans = item.Littigans;
                Category = item.Category;
                Header = item.Header;
                Note = item.Note;
                if (item.PrisonDate > System.Convert.ToDateTime("01.01.2000 00:00:00"))
                {
                    PrisonDate = $"Дата содержания под стражей: {item.PrisonDate.ToShortDateString()}";
                }
                Title = Header;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async Task ExecuteLoadHearingCommand(string _id)
        {
            IsBusy = true;

            try
            {
                ItemsHearing.Clear();
                var items = await App.DataBase.GetCourtsAsync(_id);
                items = items.OrderByDescending(x => x.Date).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        ItemsHearing.Add(item);
                    }
                }

                CourtHearingSize = ItemsHearing.Count * 75;
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

        async Task ExecuteLoadDecisionsCommand(string _id)
        {
            IsBusy = true;

            try
            {
                ItemsDecision.Clear();
                var items = await App.DataBase.GetDecisionByCaseAsync(_id);
                items = items.OrderByDescending(x => x.Date).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        Debug.WriteLine(item.Date);
                        Debug.WriteLine(item.Court);
                        ItemsDecision.Add(item);
                    }
                }

                CourtDecisionSize = ItemsDecision.Count * 130;
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

        private void OnAddDateItem(object obj)
        {
            AddDateAsync();
        }

        private async void AddDateAsync()
        {
            var item = await App.DataBase.GetCasesByCaseAsync(id);

            string result = await Shell.Current.DisplayPromptAsync("Добавить дату заседания", $"Введите дату заседания для {Header}:", maxLength: 16);
            if (TextManager.DateTime(result))
            {
                if (Convert.ToDateTime(result) >= DateTime.Now)
                {
                    Courts _courts = new Courts
                    {
                        Date = Convert.ToDateTime(result),
                        Case = item.Case,
                        Judge = item.Judge,
                        Court = item.Court,
                        Littigans = item.Littigans,
                        Category = item.Category,
                        Status = "local",
                        SoketHeader = "",
                        SoketNote = "",
                        SoketPrisonDate = ""
                    };

                    try
                    {
                        await App.DataBase.SaveCourtsAsync(_courts);
                        FileManager.WriteLog("add date", _courts.Case, _courts.Date.ToString());
                        await Shell.Current.DisplayAlert("Успешно", "Дата заседания добавлена", "OK");
                    }
                    catch (Exception e)
                    {
                        await Shell.Current.DisplayAlert("Ошибка", e.Message, "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Дата не может быть в прошлом", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не верно введен формат даты и времени", "OK");
            }


            //Device.BeginInvokeOnMainThread(async () => await Shell.Current.DisplayAlert("Alert", "your message", "OK"));

        }

        private void OnAddDatePrisonItem(object obj)
        {
            AddDatePrisonAsync();
        }

        private async void AddDatePrisonAsync()
        {
            
            string result = await Shell.Current.DisplayPromptAsync("Добавить дату содержания под стражей", $"Введите дату заседания для {Header}:", maxLength: 10);

            if (TextManager.Date(result))
            {
                if (Convert.ToDateTime(result) >= DateTime.Now)
                {
                    var item = await App.DataBase.GetCasesByCaseAsync(id);
                    if (item != null)
                    {
                        item.PrisonDate = Convert.ToDateTime(result);
                        await App.DataBase.UpdateCasesAsync(item);
                        FileManager.WriteLog("add prison date", item.Case, item.PrisonDate.ToString());
                        await Shell.Current.DisplayAlert("Успешно", "Дата содержания под стражей добавлена", "OK");
                    }                
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Дата не может быть в прошлом", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Не верно введен формат даты", "OK");
            }

            //Device.BeginInvokeOnMainThread(async () => await Shell.Current.DisplayAlert("Alert", "your message", "OK"));
        }

        private void DeleteItem(object obj)
        {
            DeleteAsync();
        }

        private async void DeleteAsync()
        {
            var item = await App.DataBase.GetCasesByCaseAsync(id);

            bool result = await Shell.Current.DisplayAlert("Удаление", "Вы хотите удалить судебное дело?", "Yes", "No");

            if (result)
            {
                await App.DataBase.DeleteCasesAsync(item);
                FileManager.WriteLog("delete case", item.Case, "");
                await Shell.Current.DisplayAlert("Успешно", "Судебное дело удалено", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private void DeleteHearingItem(object obj)
        {
            DeleteHearingAsync();
        }

        private async void DeleteHearingAsync()
        {
            var item = await App.DataBase.GetCourtsAsync(id);

            if (item.Count > 0)
            {
                var _item = item.Where(x => x.Status == "local").ToList();

                if (_item.Count > 0)
                {
                    string _date = _item.LastOrDefault().Date.ToString();
                    await App.DataBase.DeleteCourtsAsync(_item.LastOrDefault());
                    FileManager.WriteLog("delete hearing", _item.LastOrDefault().Case, _date);
                    await Shell.Current.DisplayAlert("Успешно", $"Заседание удалено {_date}", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", $"Заседания отсутствуют", "OK");

                }
            }
        }

        private void EditNote(object obj)
        {
            EditNoteAsync();
        }

        private async void EditNoteAsync()
        {
            
            string result = await Shell.Current.DisplayPromptAsync("Редактировать заметки", $"Редактирование заметок для {Header}:", maxLength: 120, initialValue:Note);
            Debug.WriteLine(result);
            if (!String.IsNullOrWhiteSpace(result))
            {
                try
                {
                    var _cases = await App.DataBase.GetCasesByCaseAsync(id);
                    _cases.Note = result;
                    await App.DataBase.UpdateCasesAsync(_cases);
                    await Shell.Current.DisplayAlert("Успешно", $"Заметка обновлена", "OK");
                    Note = result;
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Ошибка", $"Ошибка обновления заметки", "OK");
                }
            
            }                                 
        }

        async void OnItemDecisonsSelected(Decision _item)
        {
            Debug.WriteLine(_item);

            if (_item == null)
                return;

            try
            {
                Debug.WriteLine(_item);
                await Browser.OpenAsync("https://reyestr.court.gov.ua/Review/" + _item.Id, BrowserLaunchMode.SystemPreferred);
            }
            catch
            {
     
            }
        }

        #endregion
    }
}
