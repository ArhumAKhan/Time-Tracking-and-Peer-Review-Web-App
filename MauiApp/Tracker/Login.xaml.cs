using MySql.Data.MySqlClient;
using System;
using Microsoft.Maui.Controls;

namespace Tracker
{
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string netId = NetIdEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(netId) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both Net ID and Password.", "OK");
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    // Query to authenticate the user and get the utd_id
                    string query = "SELECT utd_id FROM users WHERE net_id = @netId AND password = @password AND user_type = 'P'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@netId", netId);
                        command.Parameters.AddWithValue("@password", password);

                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // User authenticated, get the utd_id
                            int utdId = Convert.ToInt32(result);

                            // Navigate to the ClassListPage and pass the utd_id
                            await Navigation.PushAsync(new ClassList(utdId));
                        }
                        else
                        {
                            // Authentication failed
                            await DisplayAlert("Error", "Invalid Net ID or Password.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors in the login process
                await DisplayAlert("Error", "Unable to login: " + ex.Message, "OK");
            }
        }
    }
}
