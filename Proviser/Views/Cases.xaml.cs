using Proviser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cases : ContentPage
    {
        CasesViewModel _viewModel;
        public Cases()
        {
            InitializeComponent();

            BindingContext = _viewModel = new CasesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}