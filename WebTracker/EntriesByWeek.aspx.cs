using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace YourNamespace
{
    public partial class EntriesByWeek : System.Web.UI.Page
    {
        private decimal totalHoursLogged = 0;
        private string utdId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
            {
                string week = Request.QueryString["week"];

                if (week == "previous")
                {
                    LoadTimeEntriesForPreviousWeek();
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
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // Start of current week (Monday)
            DateTime endOfWeek = startOfWeek.AddDays(6); // End of current week (Sunday)

            LoadTimeEntries(startOfWeek, endOfWeek);
        }

        private void LoadTimeEntriesForPreviousWeek()
        {
            // Calculate the start and end dates of the previous week (Monday to Sunday)
            DateTime today = DateTime.Now;
            DateTime startOfPreviousWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday - 7); // Start of previous week (Monday)
            DateTime endOfPreviousWeek = startOfPreviousWeek.AddDays(6); // End of previous week (Sunday)

            LoadTimeEntries(startOfPreviousWeek, endOfPreviousWeek);
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
                                     WHERE log_date BETWEEN @startDate AND @endDate and utd_id = @utdId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@utdId", utdId);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the GridView
                    gvTimeEntries.DataSource = dt;
                    gvTimeEntries.DataBind();
                }
                catch (Exception ex)
                {
                    // Handle errors (for example, log them)
                    // Display error message (optional)
                }
            }
        }

        protected void gvTimeEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Calculate the total hours logged
                decimal hours = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "hours_logged"));
                totalHoursLogged += hours;
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                // Display the total hours in the footer
                Label lblTotalHours = (Label)e.Row.FindControl("lblTotalHours");
                if (lblTotalHours != null)
                {
                    lblTotalHours.Text = totalHoursLogged.ToString("F2"); // Display total hours as decimal
                }
            }
        }
    }
}
    
