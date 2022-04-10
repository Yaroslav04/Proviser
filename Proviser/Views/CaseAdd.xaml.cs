using Proviser.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CaseAdd : ContentPage
    {
        public CaseAdd()
        {
            InitializeComponent();
            BindingContext = new CaseAddViewModel();
        }
    }
}