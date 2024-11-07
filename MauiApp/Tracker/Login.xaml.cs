using MySql.Data.MySqlClient;
using System;
using Microsoft.Maui.Controls;

namespace Tracker
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

    public partial class Login : ContentPage
    {
        // ** Constructor **
        // Initializes the login page and its components.
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

            // ** Input Validation **
            // Check if the NetID or password fields are empty and display an error if so.
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

                    // ** SQL Query to Authenticate User **
                    // Query to verify NetID and password, specifically for user type 'P'
                    string query = "SELECT utd_id FROM users WHERE net_id = @netId AND password = @password AND user_type = 'P'";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@netId", netId);
                        command.Parameters.AddWithValue("@password", password);

                        var result = command.ExecuteScalar();

                        // ** Authentication Check **
                        // If the query returns a value, authentication is successful, and we retrieve utd_id.
                        if (result != null)
                        {
                            // User authenticated, retrieve utd_id
                            int utdId = Convert.ToInt32(result);

                            // ** Navigation **
                            // Navigate to ClassListPage and pass the retrieved utd_id
                            await Navigation.PushAsync(new ClassList(utdId));
                        }
                        else
                        {
                            // Authentication failed, show error message
                            await DisplayAlert("Error", "Invalid Net ID or Password.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ** Error Handling **
                // Display an error message if there’s an issue during the login process
                await DisplayAlert("Error", "Unable to login: " + ex.Message, "OK");
            }
        }
    }
}
