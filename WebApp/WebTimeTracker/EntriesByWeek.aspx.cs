using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using Org.BouncyCastle.Utilities.Collections;
using System.EnterpriseServices.CompensatingResourceManager;

namespace YourNamespace
{
    public partial class EntriesByWeek : System.Web.UI.Page
    {
        private int totalHoursLogged = 0; // Store total hours
        private int totalMinutesLogged = 0; // Store total minutes
        private string utdId;
        private string courseId = "cs4485";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if utd_id is stored in the session
                if (Session["utd_id"] != null)
                {
                    utdId = Session["utd_id"].ToString();
                }
                else
                {
                    // If utd_id is not found in session, redirect back to login page
                    Response.Redirect("Login.aspx");
                }
                string week = Request.QueryString["week"];

                if (week == "previous")
                {
                    LoadTimeEntriesForPreviousWeek();
                }
                else if (week == "all")
                {
                    LoadTimeEntriesForEntireProject(); // Load all entries
                }
                else
                {
                    LoadTimeEntriesForCurrentWeek();
                }
            }
        }

        private void LoadTimeEntriesForCurrentWeek()
        {
            // Calculate the start and end dates of the current week (Monday to Sunday)
            DateTime today = DateTime.Now;
            int daysSinceMonday = (today.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)today.DayOfWeek - 1;
            DateTime startOfWeek = today.AddDays(-daysSinceMonday); // Start of current week (Monday)
            DateTime endOfWeek = startOfWeek.AddDays(6); // End of current week (Sunday)

            // Set label to indicate it's the current week
            lblWeekInfo.Text = $"Current Week: {startOfWeek.ToString("MM/dd/yyyy")} - {endOfWeek.ToString("MM/dd/yyyy")}";

            // Load the entries for the calculated week
            LoadTimeEntries(startOfWeek, endOfWeek);
        }

        private void LoadTimeEntriesForPreviousWeek()
        {
            // Calculate the start and end dates of the previous week (Monday to Sunday)
            DateTime today = DateTime.Now;
            int daysSinceMonday = (today.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)today.DayOfWeek - 1;
            DateTime startOfPreviousWeek = today.AddDays(-daysSinceMonday - 7); // Start of previous week (Monday)
            DateTime endOfPreviousWeek = startOfPreviousWeek.AddDays(6); // End of previous week (Sunday)

            // Set label to indicate it's the previous week
            lblWeekInfo.Text = $"Previous Week: {startOfPreviousWeek.ToString("MM/dd/yyyy")} - {endOfPreviousWeek.ToString("MM/dd/yyyy")}";

            // Load the entries for the calculated week
            LoadTimeEntries(startOfPreviousWeek, endOfPreviousWeek);
        }

        // New method to load all entries for the entire project
        private void LoadTimeEntriesForEntireProject()
        {
            // Set label to indicate it's the entire project
            lblWeekInfo.Text = "Entire Project: ";

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to retrieve all time entries for the entire project
                    string query = @"SELECT log_id, utd_id, course_id, log_date, hours_logged, work_desc FROM time_logs 
                                    WHERE  utd_id = @utdId AND course_id = @courseId ORDER BY log_date";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@utdId", utdId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the GridView
                    gvTimeEntries.DataSource = dt;
                    gvTimeEntries.DataBind();

                    calculateTotalHours(dt);

                    // Set the total hours and minutes in the label
                    lblTotalHours.Text = $"Total Hours Logged: {totalHoursLogged} hours, {totalMinutesLogged} minutes";
                }
                catch (Exception ex)
                {
                    // Handle errors (for example, log them)
                    // Display error message (optional)
                }
            }
        }

        private void LoadTimeEntries(DateTime startDate, DateTime endDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to retrieve time entries for the specified week
                    string query = @"SELECT log_id, utd_id, course_id, log_date, hours_logged, work_desc 
                                     FROM time_logs
                                     WHERE log_date BETWEEN @startDate AND @endDate
                                        AND utd_id = @utdId AND course_id = @courseId 
                                      ORDER BY log_date";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@utdId", utdId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);



                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the GridView
                    gvTimeEntries.DataSource = dt;
                    gvTimeEntries.DataBind();

                    calculateTotalHours(dt);

                    // Set the total hours and minutes in the label
                    lblTotalHours.Text = $"Total Hours Logged: {totalHoursLogged} hours, {totalMinutesLogged} minutes";
                }
                catch (Exception ex)
                {
                    // Handle errors (for example, log them)
                    // Display error message (optional)
                }
            }
        }

        private void calculateTotalHours(DataTable dt)
        {
            totalHoursLogged = 0;
            totalMinutesLogged = 0;

            // Calculate total hours and minutes
            foreach (DataRow row in dt.Rows)
            {
                decimal totalDecimalHours = Convert.ToDecimal(row["hours_logged"]);
                int hours = (int)Math.Floor(totalDecimalHours); // Extract hours as an integer
                int minutes = (int)Math.Floor((totalDecimalHours - hours) * 60); // Extract minutes as an integer

                totalHoursLogged += hours;
                totalMinutesLogged += minutes;
            }

            // Convert accumulated minutes to hours if total minutes are 60 or more
            if (totalMinutesLogged >= 60)
            {
                totalHoursLogged += totalMinutesLogged / 60; // Add additional hours
                totalMinutesLogged = totalMinutesLogged % 60; // Remainder minutes
            }

        }

        // Event handler for RowDataBound event to calculate the total logged hours
        //protected void gvTimeEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    //if (e.Row.RowType == DataControlRowType.DataRow)
        //    //{
        //    //    // Calculate the total hours logged
        //    //    decimal totalDecimalHours = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "hours_logged"));
        //    //    decimal hours = Math.Floor(totalDecimalHours); // Extract hours
        //    //    decimal minutes = (totalDecimalHours - hours) * 60; // Extract minutes

        //    //    totalHoursLogged += hours;
        //    //    totalMinutesLogged += minutes;
        //    //}
        //}
    }
}
