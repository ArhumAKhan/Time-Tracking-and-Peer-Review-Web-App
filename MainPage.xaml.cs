using Microsoft.Maui.Controls; // This is necessary to resolve 'ContentPage', 'BindingContext', etc.
using TimeTrackApp; // Make sure this matches the actual namespace where MainPageViewModel is defined.

namespace TimeTrackApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            // Initialize the XAML components defined in MainPage.xaml
            InitializeComponent();

            // Set the BindingContext to the ViewModel
            BindingContext = new MainPageViewModel();
        }
    }
}
