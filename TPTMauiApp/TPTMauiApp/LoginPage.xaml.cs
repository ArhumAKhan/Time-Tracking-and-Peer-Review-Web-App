using System.Text.RegularExpressions;

namespace TPTMauiApp
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string netID = NetIdEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(netID) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both NetID and Password.", "OK");
                return;
            }

            string pattern = @"^[A-Za-z]{3}[0-9]{6}$";
            if (!Regex.IsMatch(netID, pattern))
            {
                await DisplayAlert("Error", "Invalid NetID.", "OK");
                return;
            }else
            {
                await Navigation.PushAsync(new MainPage());
            }
        }
    }
}
