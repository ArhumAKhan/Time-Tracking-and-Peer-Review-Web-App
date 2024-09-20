using System.Text.RegularExpressions;

namespace TPTMauiApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            string netID = NetIdEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(netID) || string.IsNullOrEmpty(password))
            {
                DisplayAlert("Error", "Please enter both NetID and Password.", "OK");
                return;
            }

            string pattern = @"^[A-Za-z]{3}[0-9]{6}$";
            if (!Regex.IsMatch(netID, pattern))
            {
                DisplayAlert("Error", "Invalid NetID.", "OK");
                return;
            }

            // placeholder for actual login logic
            DisplayAlert("Success", "Login successful!", "OK");
        }
    }
}
