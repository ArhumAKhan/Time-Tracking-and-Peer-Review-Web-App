using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace WebTracker
{
    // ******************************************************************************
    // * Login Page for WebTracker Application
    // *
    // * Written by Nikhil Giridharan for CS 4485.
    // * NetID: nxg220038
    // *
    // * This page allows users to log in by verifying their NetID and password against
    // * stored database credentials. Successful login redirects users to the TimeEntry page,
    // * where further actions can be performed based on their session data.
    // * 
    // ******************************************************************************
    public partial class Login : System.Web.UI.Page
    {
        // ** Page Load Event **
        // This method is called every time the page is loaded or refreshed.
        protected void Page_Load(object sender, EventArgs e)
        {
            // Any page load logic (if needed)
        }

        // ** Login Button Click Event **
        // This method is executed when the Login button is clicked. It verifies the user's NetID and password against
        // the database. If successful, it retrieves the user's UTD ID and stores it in a session variable, then redirects
        // to the TimeEntry page. Otherwise, it displays an error message.
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Get user input from text fields
            string netID = txtNetID.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Retrieve connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            // Establish a MySQL database connection using the connection string
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open the connection to the database
                    conn.Open();

                    // ** SQL Query to Verify User Credentials **
                    // Prepare a SQL command to check if a matching NetID and password exist in the 'users' table
                    string query = "SELECT COUNT(1) FROM users WHERE net_id = @net_id AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Add parameters to prevent SQL injection attacks
                    cmd.Parameters.AddWithValue("@net_id", netID);
                    cmd.Parameters.AddWithValue("@password", password);

                    // Execute the command and get the result (1 if credentials match, 0 otherwise)
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    // ** Check Login Success **
                    // If a matching record is found, proceed with session setup and redirection
                    if (count == 1)
                    {
                        // ** Retrieve UTD ID for Session Storage **
                        // Query the UTD ID based on NetID to store in session for use on the TimeEntry page
                        cmd.CommandText = "SELECT utd_id FROM users WHERE net_id = @net_id";
                        int utdId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Store the user's UTD ID and NetID in session for later access
                        Session["utd_id"] = utdId;
                        Session["net_id"] = netID;

                        // Redirect to the TimeEntry page upon successful login
                        Response.Redirect("TimeEntry.aspx");
                    }
                    else
                    {
                        // ** Invalid Credentials **
                        // Display an error message if NetID or password is incorrect
                        lblMessage.Text = "Invalid Net ID or Password!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    // ** Error Handling **
                    // Display an error message with exception details if an error occurs during login
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}
