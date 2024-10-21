using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

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
                Authenticate(netID, password);
            }
        }

        private async void Authenticate(string netID, string password)
        {
            string connectionString = "server=localhost;user=root;database=team73_db;port=3306;password=team73";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT utd_id FROM users WHERE net_id = @netID AND password = @password AND user_type = @userType";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@netID", netID);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@userType", 'S');

                        var result = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                        if (result > 0)
                        {
                            await Navigation.PushAsync(new MainPage());
                        }
                        else
                        {
                            await DisplayAlert("Error", "Invalid NetID or Password.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
