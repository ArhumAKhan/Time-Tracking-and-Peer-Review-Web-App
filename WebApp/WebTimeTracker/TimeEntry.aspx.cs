using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WebTracker
{
    // ******************************************************************************
    // * Time Entry Page for WebTracker Application
    // *
    // * Written by Nikhil Giridharan for CS 4485.
    // * NetID: nxg220038
    // *
    // * This page allows users to enter and submit their logged hours for a specified
    // * course and date. The page validates time format, checks if a log entry exists,
    // * and performs an update or insert operation in the 'time_logs' table.
    // * 
    // ******************************************************************************
    public partial class TimeEntry : System.Web.UI.Page
    {
        // ** Page Load Event **
        // This method runs when the page loads and performs setup tasks such as setting
        // the date range and checking if the user is logged in (UTD ID stored in session).
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if student_id is stored in the session
                if (Session["student_id"] != null)
                {
                    studentId.Text = Session["student_id"].ToString();
                }
                else
                {
                    // If student_id is not found in session, redirect back to login page
                    Response.Redirect("Login.aspx");
                }

                // ** Set Date Range **
                // Set entry date to today and restrict date range to the last 3 days.
                entryDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                entryDate.Attributes["min"] = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
                entryDate.Attributes["max"] = DateTime.Now.ToString("yyyy-MM-dd");

                // Enable submit button based on description length
                entryDescription.Attributes["oninput"] = "enableSubmitButton();";
            }
        }

        // ** Submit Button Click Event **
        // This method runs when the user clicks the Submit button. It validates the time format, 
        // retrieves the data from form fields, and then either inserts a new log entry or updates an
        // existing one based on the user's UTD ID, course ID, and date.
        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Retrieve form data
                int studentIdValue = int.Parse(this.studentId.Text);
                string courseIdValue = Session["course_id"].ToString();
                DateTime entryDateValue = DateTime.Parse(entryDate.Text);
                string timeInput = hoursLogged.Text; // Assuming `hoursLogged` is now a TextBox for HH:MM input
                string workDesc = entryDescription.Text;

                // ** Validate HH:MM Time Format **
                // Check if the time input matches the HH:MM format using a regular expression.
                string timePattern = @"^(?:[01]?\d|2[0-3]):[0-5]\d$";
                if (Regex.IsMatch(timeInput, timePattern))
                {
                    // Parse hours and minutes from the input
                    string[] timeParts = timeInput.Split(':');
                    int hours = int.Parse(timeParts[0]);
                    int minutes = int.Parse(timeParts[1]);
                    minutes += hours * 60;

                    // Connection string from Web.config
                    string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            // Open connection to the MySQL database
                            conn.Open();

                            // ** Check for Existing Log Entry **
                            // Query to check if an entry already exists for the specified utd_id, date, and course_id
                            string checkQuery = "SELECT COUNT(1) FROM time_logs WHERE student_id = @student_id AND log_date = @log_date AND course_id = @course_id";
                            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                            checkCmd.Parameters.AddWithValue("@student_id", studentIdValue);
                            checkCmd.Parameters.AddWithValue("@log_date", entryDateValue);
                            checkCmd.Parameters.AddWithValue("@course_id", courseIdValue);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            // ** Update or Insert Log Entry **
                            // Update existing log entry if it exists; otherwise, insert a new record.
                            if (count > 0)
                            {
                                // ** Update Existing Log Entry **
                                string updateQuery = "UPDATE time_logs SET course_id = @course_id, minutes_logged = @minutes, work_desc = @work_desc " +
                                                     "WHERE student_id = @student_id AND log_date = @log_date";
                                MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                                updateCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                                updateCmd.Parameters.AddWithValue("@minutes", minutes);
                                updateCmd.Parameters.AddWithValue("@work_desc", workDesc);
                                updateCmd.Parameters.AddWithValue("@student_id", studentIdValue);
                                updateCmd.Parameters.AddWithValue("@log_date", entryDateValue);

                                updateCmd.ExecuteNonQuery();
                                lblMessage.Text = "Time log updated successfully!";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                // ** Insert New Log Entry **
                                string insertQuery = "INSERT INTO time_logs (student_id, course_id, log_date, minutes_logged, work_desc) " +
                                                     "VALUES (@student_id, @course_id, @log_date, @minutes, @work_desc)";
                                MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                                insertCmd.Parameters.AddWithValue("@student_id", studentIdValue);
                                insertCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                                insertCmd.Parameters.AddWithValue("@log_date", entryDateValue);
                                insertCmd.Parameters.AddWithValue("@minutes", minutes);
                                insertCmd.Parameters.AddWithValue("@work_desc", workDesc);

                                insertCmd.ExecuteNonQuery();
                                lblMessage.Text = "Time log submitted successfully!";
                                lblMessage.ForeColor = System.Drawing.Color.Green;
                            }
                        }
                        catch (Exception ex)
                        {
                            // ** Error Handling **
                            // Display an error message with exception details if an error occurs during the database operation.
                            lblMessage.Text = "Error: " + ex.Message;
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                else
                {
                    // ** Invalid Time Format Message **
                    lblMessage.Text = "Invalid time format. Please use HH:MM.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }
    }
}
