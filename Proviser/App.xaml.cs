using Proviser.Data;
using Proviser.Servises;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser
{
    public partial class App : Application
    {

        static DataBase dataBase;
        public static DataBase DataBase
        {
            get
            {
                if (dataBase == null)
                {
                    dataBase = new DataBase(FileManager.GeneralPath(), new List<string> { "CourtsDataBase.db3", "CasesDataBase.db3", "DecisionDataBase.db3" });
                }
                return dataBase;
            }
        }

        public App()
        {
            InitializeComponent();                
            MainPage = new AppShell();
        }
   
        protected override void OnStart()
        {
            Debug.WriteLine("start");
        }

        protected override void OnSleep()
        {
            Debug.WriteLine("sleep");
        }

        protected override void OnResume()
        {
            Debug.WriteLine("resume");
        }
    }
}
