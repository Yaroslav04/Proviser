using Proviser.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Case : ContentPage
    {
        public Case()
        {
            InitializeComponent();
            BindingContext = new CaseViewModel();
        }
    }
}