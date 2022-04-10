using Proviser.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourtsHearing : ContentPage
    {
        CourtsHearingViewModel _viewModel;

        public CourtsHearing()
        {
            InitializeComponent();

            BindingContext = _viewModel = new CourtsHearingViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

    }
}