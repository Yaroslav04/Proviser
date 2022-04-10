using Proviser.Models;
using Proviser.Services;
using Proviser.Servises;
using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace Proviser.ViewModels
{
    public class CaseAddViewModel : BaseViewModel
    {
        public CaseAddViewModel()
        {
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        #region Properties

        private string caseId;
        private string judge;
        private string court;
        private string littigans;
        private string category;
        private string header;
        private string note;
        private DateTime prisonDate;

        public string Case
        {
            get => caseId;
            set
            {
                SetProperty(ref caseId, value);
                if (caseId.Length > 5)
                {
                    LoadCourtsToFields(caseId);
                }
            }
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
            set
            {
                SetProperty(ref littigans, value);
                Debug.WriteLine(littigans);
            }
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

        public DateTime PrisonDate
        {
            get => prisonDate;
            set => SetProperty(ref prisonDate, value);
        }

        #endregion

        #region Commands
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        #endregion

        #region Functions

        private void OnCancel()
        {
            Case = "";
            Clear();
        }

        private async void OnSave()
        {
            var _list = await App.DataBase.GetCourtsAsync(Case);
            if (_list.Count > 0)
            {
                Courts courts = _list.LastOrDefault();
                Cases cases = new Cases()
                {
                    Case = courts.Case,
                    Judge = courts.Judge,
                    Court = courts.Court,
                    Littigans = courts.Littigans,
                    Category = courts.Category,
                    Header = TextManager.Header(courts.Littigans),
                    Note = Note,
                    PrisonDate = System.Convert.ToDateTime("01.01.2000 00:00:00")
                };

                Case = "";
                Clear();

                try
                {
                    await App.DataBase.SaveCasesAsync(cases);
                    FileManager.WriteLog("add case", cases.Case, "");
                    await Shell.Current.DisplayAlert("Успешно", "Зарегистритровано", "OK");
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Уже зарегистрировано", "OK");
                    
                }
            }
            else
            {
                Cases cases = new Cases()
                {
                    Case = Case,
                    Judge = Judge,
                    Court = Court,
                    Littigans = Littigans,
                    Category = Category,
                    Header = Header,
                    Note = Note,
                    PrisonDate = System.Convert.ToDateTime("01.01.2000 00:00:00")
                };

                Case = "";
                Clear();

                try
                {
                    await App.DataBase.SaveCasesAsync(cases);
                    FileManager.WriteLog("add case", cases.Case, "");
                    await Shell.Current.DisplayAlert("Успешно", "Зарегистритровано", "OK");
                }
                catch
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Уже зарегистрировано", "OK");
                }
            }
        }

        private async void LoadCourtsToFields(string _case)
        {
            var _list = await App.DataBase.GetCourtsAsync(_case);
            if (_list.Count > 0)
            {
                Courts courts = _list.LastOrDefault();
                if (courts != null)
                {
                    Judge = courts.Judge;
                    Court = courts.Court;
                    Littigans = courts.Littigans;
                    Category = courts.Category;
                    Header = TextManager.Header(courts.Littigans);
                }
                else
                {
                    Clear();
                }
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            Judge = "";
            Court = "";
            Littigans = "";
            Category = "";
            Header = "";
            Note = "";
        }

        #endregion
    }
}
