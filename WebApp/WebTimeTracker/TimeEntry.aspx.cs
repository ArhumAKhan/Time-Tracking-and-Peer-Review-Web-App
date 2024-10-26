using MySql.Data.MySqlClient;
using System;
using System.Configuration;

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

                // Populate hours and minutes dropdowns (same logic as before)
                for (int i = 0; i <= 24; i++)
                {
                    hoursLogged.Items.Add(i.ToString());
                }

                for (int i = 0; i < 60; i += 15)
                {
                    minutesLogged.Items.Add(i.ToString().PadLeft(2, '0'));
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
                int hours = int.Parse(hoursLogged.SelectedValue);
                int minutes = int.Parse(minutesLogged.SelectedValue);
                string workDesc = entryDescription.Text;

                // Convert hours and minutes into decimal
                decimal totalHoursLogged = hours + (minutes / 60m);

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
                            string updateQuery = "UPDATE time_logs SET course_id = @course_id, hours_logged = @hours_logged, work_desc = @work_desc " +
                                                 "WHERE utd_id = @utd_id AND log_date = @log_date";
                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                            updateCmd.Parameters.AddWithValue("@hours_logged", totalHoursLogged);
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
                            string insertQuery = "INSERT INTO time_logs (utd_id, course_id, log_date, hours_logged, work_desc) " +
                                                 "VALUES (@utd_id, @course_id, @log_date, @hours_logged, @work_desc)";
                            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                            insertCmd.Parameters.AddWithValue("@utd_id", utdIdValue);
                            insertCmd.Parameters.AddWithValue("@course_id", courseIdValue);
                            insertCmd.Parameters.AddWithValue("@log_date", entryDateValue);
                            insertCmd.Parameters.AddWithValue("@hours_logged", totalHoursLogged);
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
        }
    }
}
