using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace YourNamespace
{
    public partial class TimeEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if utd_id is stored in the session
                if (Session["utd_id"] != null)
                {
                    utdId.Text = Session["utd_id"].ToString();
                }
                else
                {
                    // If utd_id is not found in session, redirect back to login page
                    Response.Redirect("Login.aspx");
                }

                // Set date range (last 3 days)
                entryDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                entryDate.Attributes["min"] = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                entryDate.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");

                // Enable submit button based on description length
                entryDescription.Attributes["oninput"] = "enableSubmitButton();";
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Retrieve form data
                int utdIdValue = int.Parse(utdId.Text);
                string courseIdValue = courseId.Text;
                DateTime entryDateValue = DateTime.Parse(entryDate.Text);
                string timeInput = hoursLogged.Text; // Assuming `hoursLogged` is now a TextBox for HH:MM input
                string workDesc = entryDescription.Text;

                // Validate and parse HH:MM input
                string timePattern = @"^(?:[01]?\d|2[0-3]):[0-5]\d$";
                if (Regex.IsMatch(timeInput, timePattern))
                {
                    string[] timeParts = timeInput.Split(':');
                    int hours = int.Parse(timeParts[0]);
                    int minutes = int.Parse(timeParts[1]);

                    // Connection string from Web.config
                    string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            // Check if the record already exists
                            string checkQuery = "SELECT COUNT(1) FROM time_logs WHERE utd_id = @utd_id AND log_date = @log_date AND course_id = @course_id";
                            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                            checkCmd.Parameters.AddWithValue("@utd_id", utdIdValue);
                            checkCmd.Parameters.AddWithValue("@log_date", entryDateValue);
                            checkCmd.Parameters.AddWithValue("@course_id", courseIdValue);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            // If the record exists, update it; otherwise, insert a new record
                            if (count > 0)
                            {
                                // Record exists, perform update
                                string updateQuery = "UPDATE time_logs SET course_id = @course_id, hours_logged = @hours, minutes_logged = @minutes, work_desc = @work_desc " +
                                                     "WHERE utd_id = @utd_id AND log_date = @log_date";
                                MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                                updateCmd.Parameters.AddWithValue("@hours", hours);
                                updateCmd.Parameters.AddWithValue("@minutes", minutes);
                                updateCmd.Parameters.AddWithValue("@work_desc", workDesc);
                                updateCmd.Parameters.AddWithValue("@utd_id", utdIdValue);
                                updateCmd.Parameters.AddWithValue("@log_date", entryDateValue);

                                updateCmd.ExecuteNonQuery();
                                lblMessage.Text = "Time log updated successfully!";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                // Record does not exist, perform insert
                                string insertQuery = "INSERT INTO time_logs (utd_id, course_id, log_date, hours_logged, minutes_logged, work_desc) " +
                                                     "VALUES (@utd_id, @course_id, @log_date, @hours, @minutes, @work_desc)";
                                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                                insertCmd.Parameters.AddWithValue("@utd_id", utdIdValue);
                                insertCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                                insertCmd.Parameters.AddWithValue("@log_date", entryDateValue);
                                insertCmd.Parameters.AddWithValue("@hours", hours);
                                insertCmd.Parameters.AddWithValue("@minutes", minutes);
                                insertCmd.Parameters.AddWithValue("@work_desc", workDesc);

                                insertCmd.ExecuteNonQuery();
                                lblMessage.Text = "Time log submitted successfully!";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle any errors that occur during the insert/update operation
                            lblMessage.Text = "Error: " + ex.Message;
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid time format. Please use HH:MM.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}
