using MySql.Data.MySqlClient;
using System;
using Microsoft.Maui.Controls;

namespace Tracker
{
    public partial class Login : ContentPage
    {
        // ******************************************************************************
        // * Login Page for Tracker Application
        // *
        // * Written by Nikhil Giridharan and Johnny An for CS 4485.
        // * NetID: nxg220038 and hxa210014
        // *
        // * This page allows users to log in by verifying their NetID and password.
        // * Successful login retrieves the user's UTD ID and navigates to the ClassListPage,
        // * passing the UTD ID for further actions.
        // *
        // ******************************************************************************
        public Login()
        {
            InitializeComponent();
        }
        
        // ** Login Button Clicked Event **
        // This method is called when the user clicks the Login button. It verifies that both
        // NetID and password fields are not empty, then connects to the database to authenticate
        // the user. Upon successful authentication, it retrieves the user's UTD ID and navigates
        // to the ClassListPage. If authentication fails, it displays an error message.
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
                    await connection.OpenAsync();

                    // Query to authenticate the user and get the user_id
                    string query = @"
                        SELECT user_id 
                        FROM users 
                        WHERE net_id = @netId 
                        AND password = @password 
                        AND user_role = 'Professor'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@netId", netId);
                        command.Parameters.AddWithValue("@password", password);

                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            // User authenticated, get the user_id
                            int userId = Convert.ToInt32(result);

                            // Navigate to the ClassListPage and pass the user_id
                            await Navigation.PushAsync(new ClassList(userId));
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
