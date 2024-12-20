﻿using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace WebTracker
{
    // ******************************************************************************
    // * Entries By Week Page for WebTracker Application
    // *
    // * Written by Nikhil Giridharan for CS 4485.
    // * NetID: nxg220038
    // *
    // * This page displays time entries logged by a user for a specified week, previous week,
    // * current week, or the entire project. The user can view their logged hours and the
    // * application calculates total hours and minutes logged based on their entries.
    // * 
    // ******************************************************************************
    public partial class EntriesByWeek : System.Web.UI.Page
    {
        private int totalHoursLogged = 0; // Store total hours
        private int totalMinutesLogged = 0; // Store total minutes
        private string studentId; // User's UTD ID
        private string courseId ; // courseid for CS4485 from session

        // ** Page Load Event **
        // This method runs when the page loads and handles displaying entries for the current, previous week,
        // or all entries based on the "week" parameter in the query string.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if student_id  is stored in the session
                if (Session["student_id"] != null)
                {
                    studentId = Session["student_id"].ToString();
                    courseId = Session["course_id"].ToString();
                }
                else
                {
                    // If student_id is not found in session, redirect back to login page
                    Response.Redirect("Login.aspx");
                }

                // Retrieve 'week' parameter from query string to determine which data to load
                string week = Request.QueryString["week"];

                if (week == "previous")
                {
                    LoadTimeEntriesForPreviousWeek();
                }
                else if (week == "all")
                {
                    LoadTimeEntriesForEntireProject(); // Load all entries for the project
                }
                else
                {
                    LoadTimeEntriesForCurrentWeek();
                }
            }
        }

        // ** Load Current Week's Entries **
        // Loads time entries for the current week (Monday to Sunday) and binds data to the GridView.
        private void LoadTimeEntriesForCurrentWeek()
        {
            // Calculate the start and end dates of the current week
            DateTime today = DateTime.Now;
            int daysSinceMonday = (today.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)today.DayOfWeek - 1;
            DateTime startOfWeek = today.AddDays(-daysSinceMonday); // Start of current week (Monday)
            DateTime endOfWeek = startOfWeek.AddDays(6); // End of current week (Sunday)

            // Set label to indicate it's the current week
            lblWeekInfo.Text = $"Current Week: {startOfWeek:MM/dd/yyyy} - {endOfWeek:MM/dd/yyyy}";

            // Load entries for the calculated week range
            LoadTimeEntries(startOfWeek, endOfWeek);
        }

        // ** Load Previous Week's Entries **
        // Loads time entries for the previous week (Monday to Sunday) and binds data to the GridView.
        private void LoadTimeEntriesForPreviousWeek()
        {
            // Calculate the start and end dates of the previous week
            DateTime today = DateTime.Now;
            int daysSinceMonday = (today.DayOfWeek == DayOfWeek.Sunday) ? 6 : (int)today.DayOfWeek - 1;
            DateTime startOfPreviousWeek = today.AddDays(-daysSinceMonday - 7); // Start of previous week (Monday)
            DateTime endOfPreviousWeek = startOfPreviousWeek.AddDays(6); // End of previous week (Sunday)

            // Set label to indicate it's the previous week
            lblWeekInfo.Text = $"Previous Week: {startOfPreviousWeek:MM/dd/yyyy} - {endOfPreviousWeek:MM/dd/yyyy}";

            // Load entries for the calculated week range
            LoadTimeEntries(startOfPreviousWeek, endOfPreviousWeek);
        }

        // ** Load Entire Project's Entries **
        // Loads all time entries for the entire project and binds data to the GridView.
        private void LoadTimeEntriesForEntireProject()
        {
            lblWeekInfo.Text = "Entire Project: ";

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Query to retrieve all time entries for the user and specified course
                    string query = @"SELECT log_date, minutes_logged DIV 60 hours_logged, 
                                     minutes_logged % 60 minutes_logged, work_desc 
                                     FROM time_logs 
                                     WHERE student_id = @studentId AND course_id = @courseId ORDER BY log_date";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind data to the GridView
                    gvTimeEntries.DataSource = dt;
                    gvTimeEntries.DataBind();

                    // Calculate total hours and minutes from the entries
                    CalculateTotalHours(dt);

                    lblTotalHours.Text = $"Total Hours Logged: {totalHoursLogged} hours, {totalMinutesLogged} minutes";
                }
                catch (Exception ex)
                {
                    // Error handling (e.g., log errors or display to user if needed)
                    // lblMessage.Text = "Error loading data: " + ex.Message;
                    // lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        // ** Load Time Entries for Specified Date Range **
        // Loads time entries between the specified start and end dates and binds data to the GridView.
        private void LoadTimeEntries(DateTime startDate, DateTime endDate)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // SQL query to retrieve time entries for the specified date range
                    string query = @"SELECT log_date, minutes_logged DIV 60 hours_logged, 
                                     minutes_logged % 60 minutes_logged, work_desc 
                                     FROM time_logs
                                     WHERE log_date BETWEEN @startDate AND @endDate
                                        AND student_id = @studentId AND course_id = @courseId 
                                     ORDER BY log_date";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@courseId", courseId);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind data to the GridView
                    gvTimeEntries.DataSource = dt;
                    gvTimeEntries.DataBind();

                    // Calculate total hours and minutes from the entries
                    CalculateTotalHours(dt);

                    lblTotalHours.Text = $"Total Hours Logged: {totalHoursLogged} hours, {totalMinutesLogged} minutes";
                }
                catch (Exception ex)
                {
                    // Error handling (e.g., log errors or display to user if needed)
                    // lblMessage.Text = "Error loading data: " + ex.Message;
                    // lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        // ** Calculate Total Hours and Minutes **
        // Summarizes hours and minutes from the DataTable entries and updates total hours and minutes.
        private void CalculateTotalHours(DataTable dt)
        {
            totalHoursLogged = 0;
            totalMinutesLogged = 0;

            // Iterate through each entry to sum up hours and minutes
            foreach (DataRow row in dt.Rows)
            {
                int hours = Convert.ToInt32(row["hours_logged"]);
                int minutes = Convert.ToInt32(row["minutes_logged"]);

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
    }
}
