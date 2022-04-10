using Proviser.Models;
using Proviser.Services;
using Proviser.Servise;
using Proviser.Servises;
using Proviser.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Proviser
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {    
            InitializeComponent();
            FileManager.FileInit();
            _mail = FileManager.GetMail();
            _sniffer = FileManager.GetSniffer();
            Routing.RegisterRoute(nameof(CourtsHearing), typeof(CourtsHearing));
            Routing.RegisterRoute(nameof(Case), typeof(Case));
            Routing.RegisterRoute(nameof(CaseAdd), typeof(CaseAdd));

            if (FileManager.FirstStart())
            {
                RunAsyncFunctions();
                FileManager.WriteLog("system", "start", "");
            }
        }

        public async void RunAsyncFunctions()
        {
            IsBusy = true;
            await RunMailFunctions();
            await RunSnifferFunction();
            await ReminderPrisonNew();
            await ReminderPrisonRefresh();
            await ReminderHeaderRefresh();
            await Sniffer();
            await DecisonSniffer();
            IsBusy = false;
        }

        private void ServiseStartButton_Clicked(object sender, EventArgs e)
        {
            RunAsyncFunctions();
        }
        private async void MailItemClicked(object sender, EventArgs e)
        {
            if (IsBusy == false)
            {
                IsBusy = true;
                await SendMail();
                await DisplayAlert("Почта", "Отправлено", "OK");
            }
        }
        private async void DownloadItemClicked(object sender, EventArgs e)
        {      
            if (IsBusy == false)
            {
                IsBusy = true;
                await DisplayAlert("Загрузка", "Старт загрузки", "OK");
                await ImportWebHookAsync();
                IsBusy = false;
            }
        }

        #region Properties

        string _mail = "";
        List<string> _sniffer = new List<string>();

        #endregion

        #region ManualServises

        private async Task CleanDistinct()
        {
            var list = await App.DataBase.GetCourtsAsync();
            List <Courts> result = new List<Courts>();

            foreach (var item in list)
            {
                List<Courts> sublist = new List<Courts>();
                foreach (var subitem in list)
                {
                    if (item.Case == subitem.Case)
                    {
                        if (item.Date == subitem.Date)
                        {
                            sublist.Add(subitem);
                        }
                    }
                }
                if (sublist.Count > 1)
                {
                    result.AddRange(sublist);
                }
            }

            result = result.Distinct().ToList();

            foreach (var item in result)
            {
                Debug.WriteLine($"{item.Case}{item.Littigans}");
            }
        }

        #endregion

        #region Sniffer

        public async Task RunSnifferFunction()
        {
            if (_sniffer == null)
            {
                string _result = await DisplayPromptAsync("Система", "Введите свое имя фамилию и отчество", maxLength: 60);
                FileManager.SetSniffer(_result);
                _sniffer = FileManager.GetSniffer();
                await DisplayAlert("Система", "Данные для поиска установлены", "OK");
            }

        }
        public async Task Sniffer()
        {
            List<Courts> messages = new List<Courts>();
            List<string> _list = new List<string>();
            List<string> _casesString = new List<string>();

                var _cases = await App.DataBase.GetCasesAsync();
                if (_cases.Count > 0)
                {
                    foreach (var item in _cases)
                    {
                        _casesString.Add(item.Case);
                        Debug.WriteLine(item.Case);
                    }

                    if (_sniffer.Count > 0)
                    {
                        List<string> _caseResult = new List<string>();

                        foreach (string s in _sniffer)
                        {
                            List<Courts> items = await App.DataBase.GetCourtsByLittigansAsync(s);

                            if (items.Count > 0)
                            {
                                foreach (var item3 in items)
                                {
                                    Debug.WriteLine(item3.Littigans);
                                    bool sw = false;

                                    foreach (var _s in _casesString)
                                    {
                                        if (_s == item3.Case)
                                        {
                                            sw = true;
                                        }
                                    }

                                    if (sw == false)
                                    {
                                        _caseResult.Add(item3.Case);
                                        Debug.WriteLine(item3.Case);
                                    }
                                }
                            }
                        }

                        if (_caseResult.Count > 0)
                        {
                            _caseResult = _caseResult.Distinct().ToList();

                            _list = _caseResult;
                        }
                    }

                }

                if (_list.Count > 0)
                {
                    foreach (string s in _list)
                    {
                        var _c = await App.DataBase.GetCourtsAsync(s);
                        messages.Add(_c.LastOrDefault());
                    }
                }


                if (messages.Count > 0)
                {
                    foreach (var m in messages)
                    {
                        bool answer = await DisplayAlert("Поиск по прокурору", $"Найдено совпадений:\n{m.Case} {m.Littigans}\nЗарегистрировать?", "Да", "Нет");

                        if (answer)
                        {
                            var _listt = await App.DataBase.GetCourtsAsync(m.Case);
                            if (_listt.Count > 0)
                            {
                                Courts courts = _listt.LastOrDefault();
                                Models.Cases cases = new Models.Cases()
                                {
                                    Case = courts.Case,
                                    Judge = courts.Judge,
                                    Court = courts.Court,
                                    Littigans = courts.Littigans,
                                    Category = courts.Category,
                                    Header = TextManager.Header(courts.Littigans),
                                    Note = "",
                                    PrisonDate = System.Convert.ToDateTime("01.01.2000 00:00:00")
                                };
                                try
                                {
                                    await App.DataBase.SaveCasesAsync(cases);
                                    FileManager.WriteLog("add case", cases.Case, "");
                                    await Shell.Current.DisplayAlert("Успешно", "Зарегистритровано", "OK");
                                }
                                catch
                                {
                                    await Shell.Current.DisplayAlert("Ошибка", "Уже зарегистрировано или ошибка", "OK");
                                }
                            }

                        }
                    }
                }      
        }

        #endregion

        #region DecisionSniffer

        async Task DecisonSniffer()
        {
            var cases = await App.DataBase.GetCasesAsync();
            if (cases.Count > 0)
            {
                foreach (var _case in cases)
                {
                    await HTMLParser(_case.Case);
                }
                await Shell.Current.DisplayAlert("Уведомление", "Решения скачаны", "OK");
            }
        }      
        
        async Task<string> GetResponseFromCase(string _case)
        {
            HttpClient client = new HttpClient();

            List<Decision> _list = new List<Decision>();

            var values = new Dictionary<string, string> { { "CaseNumber", _case }, { "PagingInfo.ItemsPerPage", "200" }, { "Sort", "1" } };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://reyestr.court.gov.ua/", content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        async Task HTMLParser(string _case)
        {
            var text = await GetResponseFromCase(_case);
            var lines = text.Split('\n');

            List<string> container = new List<string>();
            List<Decision> list = new List<Decision>();

            bool containerSensor = false;

            foreach (string line in lines)
            {
                if (line.Contains("<tr>"))
                {
                    containerSensor = true;
                }

                if (line.Contains("</tr>"))
                {
                    container.Add(line);
                    if (container.Count > 0)
                    {
                        bool sw = false;
                        foreach (var _c in container)
                        {
                            if (_c.Contains("/Review/"))
                            {
                                sw = true;
                            }
                        }

                        if (sw)
                        {
                            Decision decision = new Decision();

                            Regex regex = new Regex(@"(/Review/)\d*");
                            MatchCollection matches = regex.Matches(container[2]);
                            if (matches.Count > 0)
                            {
                                decision.Id = matches[0].Value.Replace("/Review/", "");
                            }

                            regex = new Regex(@"(>)\w+");
                            matches = regex.Matches(container[4]);
                            if (matches.Count > 0)
                            {
                                decision.DecisionType = matches[0].Value.Replace(">", "");
                            }

                            regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d");
                            matches = regex.Matches(container[6]);
                            if (matches.Count > 0)
                            {
                                decision.Date = Convert.ToDateTime(matches[0].Value);
                            }

                            regex = new Regex(@"(>)\w+");
                            matches = regex.Matches(container[10]);
                            if (matches.Count > 0)
                            {
                                decision.JudiciaryType = matches[0].Value.Replace(">", "");
                            }

                            regex = new Regex(@"(>)\w+(/)\w+(/)\w+");
                            matches = regex.Matches(container[12]);
                            if (matches.Count > 0)
                            {
                                decision.Case = matches[0].Value.Replace(">", "");
                            }

                            decision.Court = container[14].Replace("                <td class=\"CourtName tr1\">", "").Replace("                <td class=\"CourtName tr2\">", "");

                            if (_case == decision.Case)
                            {
                                list.Add(decision);
                                Debug.WriteLine("добавили");
                            }

                        }
                    }
                    container.Clear();
                    containerSensor = false;
                }

                if (containerSensor == true)
                {
                    container.Add(line);
                }
            }

            foreach (Decision decision in list)
            {
                try
                {
                    await App.DataBase.SaveDecisionAsync(decision);
                }
                catch
                {

                }

            }
        }

        #endregion

        #region Reminder

        public async Task ReminderPrisonNew()
        {
            List<string> messages = new List<string>();

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate > DateTime.Now)
                    {
                        if ((item.PrisonDate - DateTime.Now).Days < 15)
                        {
                            if ((item.PrisonDate - DateTime.Now).Days > 10)
                            {
                                messages.Add($"Нужно продлить подстражного:\n{item.Header} {item.Case}\n{item.PrisonDate.ToShortDateString()}");
                            }
                        }
                    }
                }
                if (messages.Count > 0)
                {
                    foreach (var mes in messages)
                    {
                        await DisplayAlert("Напоминание", mes, "OK");
                    }
                }
            }
        }

        public async Task ReminderPrisonRefresh()
        {
            List<string> messages = new List<string>();

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate < DateTime.Now)
                    {
                        if ((DateTime.Now - item.PrisonDate).Days < 30)
                        {
                            messages.Add($"Нужно обновить подстражного:\n{item.Header} {item.Case}\n{item.PrisonDate.ToShortDateString()}");
                        }
                    }
                }
                if (messages.Count > 0)
                {
                    foreach (var mes in messages)
                    {
                        await DisplayAlert("Напоминание", mes, "OK");
                    }
                }
            }
        }

        public async Task ReminderHeaderRefresh()
        {
            List<string> messages = new List<string>();

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    var _courts = await App.DataBase.GetCourtsAsync(item.Case);
                    if (_courts.Count > 0)
                    {
                        if ((DateTime.Now - _courts.LastOrDefault().Date).TotalDays > 7)
                        {
                            if ((DateTime.Now - _courts.LastOrDefault().Date).TotalDays < 30)
                            {
                                messages.Add($"Нужно обновить заседание:\n{item.Header} {item.Case}\n{_courts.LastOrDefault().Date}");
                            }
                        }
                    }
                }
                if (messages.Count > 0)
                {
                    foreach (var mes in messages)
                    {
                        await DisplayAlert("Напоминание", mes, "OK");
                    }
                }
            }
        }

        #endregion

        #region Mail

        public async Task RunMailFunctions()
        {
            if (String.IsNullOrWhiteSpace(_mail))
            {

                while (String.IsNullOrWhiteSpace(_mail))
                {
                    string _result = await DisplayPromptAsync("Система", "Введите свою почту", maxLength: 60);
                    _mail = _result;
                }

                FileManager.SetMail(_mail);

                await DisplayAlert("Система", "Почта установлена", "OK");
            }

        }  
        
        public async Task SendMail()
        {
            try
            {
                ObservableCollection<Courts> Items;
                Items = new ObservableCollection<Courts>();

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

                if (Items.Count > 0)
                {
                    string _text = "";
                    foreach (var item in Items)
                    {
                        _text = _text + $"{item.Date}\n{item.SoketHeader}\n{item.Case} {item.Judge}\n";
                        if (item.SoketPrisonDate != "")
                        {
                            _text = _text + item.SoketPrisonDate + "\n";
                        }
                        if (item.SoketNote != "")
                        {
                            _text = _text + item.SoketNote + "\n";
                        }
                        _text = _text + "\n";
                    }

                    await MailServise.SendEmailAsync(_mail, _text);
                    IsBusy = false;
                }
            }
            catch
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Simple Import

        public async Task Import()
        {
            List<string> _courts = new List<string>(new string[] { "Заводський районний суд м.Дніпродзержинська", "Дніпровський районний суд м.Дніпродзержинська", "Баглійський районний суд м.Дніпродзержинська", "Дніпровський апеляційний суд" });
            using (StreamReader sr = new StreamReader(FileManager.GeneralPath("1.csv")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    foreach (string _court in _courts)
                    {
                        if (line.Contains(_court))
                        {
                            if (line.Contains("207/") | line.Contains("208/") | line.Contains("209/"))
                            {
                                try
                                {
                                    var x = TextToCourtsConverter.Transform(line);
                                    if (!x.Date.ToString().Contains("0:00:00"))
                                    {
                                        Debug.WriteLine(line);
                                        await App.DataBase.SaveCourtsAsync(x);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("format error");
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Import WebHook

        static async Task<Stream> GetDataStream(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            return await response.Content.ReadAsStreamAsync();
        }

        static async IAsyncEnumerable<string> GetDataLines(string url)
        {
            using var _stream = await GetDataStream(url);
            using (var _reader = new StreamReader(_stream))
            {
                while (!_reader.EndOfStream)
                {
                    var line = _reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    yield return line;
                }
            }
        }

        public async Task ImportWebHook()
        {
            int k = 0;
            List<string> _courts = new List<string>(new string[] { "Заводський районний суд м.Дніпродзержинська", "Дніпровський районний суд м.Дніпродзержинська", "Баглійський районний суд м.Дніпродзержинська", "Дніпровський апеляційний суд" });

            await foreach (var line in GetDataLines("https://dsa.court.gov.ua/open_data_files/91509/513/8faabdb91244be394947eb26f2153a1f.csv"))
            {

                foreach (string _court in _courts)
                {
                    if (line.Contains(_court))
                    {
                        if (line.Contains("207/") | line.Contains("208/") | line.Contains("209/"))
                        {
                            try
                            {
                                var x = TextToCourtsConverter.Transform(line);
                                if (!x.Date.ToString().Contains("0:00:00"))
                                {
                                    await App.DataBase.SaveCourtsAsync(x);
                                    k++;
                                    Debug.WriteLine(line);
                                }
                                {
                                    Debug.WriteLine("format error");
                                }                      
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            await DisplayAlert("Загрузка", "Загружено: " + k, "OK");
        }

        public Task ImportWebHookAsync()
        {
            Task t = Task.Run(() => ImportWebHook());
            return t;

        }

        #endregion

    }
}

