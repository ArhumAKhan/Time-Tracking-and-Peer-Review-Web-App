using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace YourNamespace
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Any page load logic (if needed)
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string netID = txtNetID.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Connection string from Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            // Create a connection and SQL command to verify login
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Query to check if the net_id and password match
                    string query = "SELECT COUNT(1) FROM users WHERE net_id = @net_id AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Add parameters to avoid SQL injection
                    cmd.Parameters.AddWithValue("@net_id", netID);
                    cmd.Parameters.AddWithValue("@password", password);

                    // Execute the query and get the result
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    // Check if login is successful
                    if (count == 1)
                    {
                        // Store utd_id in Session (you can retrieve it later in TimeEntry.aspx)
                        cmd.CommandText = "SELECT utd_id FROM users WHERE net_id = @net_id";
                        int utdId = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["utd_id"] = utdId;
                        Session["net_id"] = netID;

                        // Redirect to TimeEntry page
                        Response.Redirect("TimeEntry.aspx");
                    }
                    else
                    {
                        lblMessage.Text = "Invalid Net ID or Password!";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}
