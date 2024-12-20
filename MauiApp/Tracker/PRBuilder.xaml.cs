using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;

namespace Tracker
{
    // ******************************************************************************
    // * Peer Review Builder Page for Tracker Application
    // *
    // * Written by Farhat R. Kabir for CS 4485.
    // * NetID: frk200000
    // *
    // * This page allows professors to build Peer Review Forms for their classes.
    // * The page allows the user to enter a Peer Review Name, Start Date, End Date, and
    // * a list of criteria for the Peer Review. Upon submission, the Peer Review is created.
    // *
    // ******************************************************************************

    public partial class PRBuilder : ContentPage
    {
        // Id of logged in user
        private readonly int userId;

        // ** Constructor **
        // Initializes the login page and its components.
        public PRBuilder(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        // ** Add Criterion Clicked Event **
        // This method is called when the user clicks the Add Criterion button. It adds a new
        // criterion entry fields.
        private void AddCriterion_Clicked(object sender, EventArgs e)
        {
            // Create a new Label and Entry for the criteria
            CriteriaStack.Children.Add(new Label { Text = "Enter Criteria", TextColor = Colors.Black, Margin = new Thickness(20, 0, 0, 0) });
            CriteriaStack.Children.Add(new Entry { WidthRequest = 420, Margin = new Thickness(0, 0, 0, 10), Placeholder = "E.G. Attendance, Participation, etc.", TextColor = Colors.Black });
        }

        // ** Remove Criterion Clicked Event **
        // This method is called when the user clicks the Remove Criterion button. It removes
        // the last criterion entry field.
        private void RemoveCriterion_Clicked(object sender, EventArgs e)
        {
            // Only remove if there is more than one criterion entry (with its label)
            if (CriteriaStack.Children.Count > 2)
            {
                CriteriaStack.Children.RemoveAt(CriteriaStack.Children.Count - 1);
                CriteriaStack.Children.RemoveAt(CriteriaStack.Children.Count - 1);
            }
        }

        // ** Submit Clicked Event **
        // Form submission method for creating a Peer Review. It retrieves data inputted
        // by the user, creates a new Peer Review in the database, and adds the criteria
        // to the Peer Review.
        private void Submit_Clicked(object sender, EventArgs e)
        {
            string startDate = StartDateEntry.Date.ToString("yyyy-MM-dd");
            string endDate = EndDateEntry.Date.ToString("yyyy-MM-dd");
            string prName = PRNameEntry.Text;

            List<string> criteria_list = [];

            foreach (var child in CriteriaStack.Children)
            {
                if (child is Entry entry)
                {
                    System.Diagnostics.Debug.WriteLine(entry.Text);
                    criteria_list.Add(entry.Text);
                }
            }

            using MySqlConnection connection = new (DatabaseConfig.ConnectionString);

            connection.Open();

            string pr_course = "SELECT c.course_id FROM courses AS c, users AS u JOIN professors AS p ON u.user_id = p.user_id " + 
                               "WHERE p.user_id = @user_id AND c.professor_id = p.professor_id";

            using MySqlCommand course_command = new(pr_course, connection);

            course_command.Parameters.AddWithValue("@user_id", userId);

            int courseId = (int)course_command.ExecuteScalar();

            System.Diagnostics.Debug.WriteLine("Course ID: " + courseId);

            // Insert Peer Review into the database
            string pr_insert = "INSERT INTO peer_review (start_date, end_date, course_id, pr_name) " +
                               "VALUES (@startDate, @endDate, @course_id, @pr_name);";

            using MySqlCommand command = new (pr_insert, connection);
            
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);
            command.Parameters.AddWithValue("@course_id", courseId);
            command.Parameters.AddWithValue("@pr_name", prName);

            command.ExecuteNonQuery();

            int pr_id = (int)command.LastInsertedId;
            System.Diagnostics.Debug.WriteLine("Last Inserted ID: " + pr_id);

            // Add each criterion to the Peer Review
            foreach (string criteria in criteria_list)
            {
                string criteria_insert = "INSERT INTO pr_criteria (pr_id, criteria_desc) VALUES (@pr_id, @criteria);";

                using MySqlCommand criteria_command = new (criteria_insert, connection);

                criteria_command.Parameters.AddWithValue("@pr_id", pr_id);
                criteria_command.Parameters.AddWithValue("@criteria", criteria);

                criteria_command.ExecuteNonQuery();
            }

            connection.Close();

            // Display success message
            DisplayAlert("Success", "Peer Review Created Successfully", "OK");

            // Clear the form fields
            PRNameEntry.Text = string.Empty;
            StartDateEntry.Date = DateTime.Now;
            EndDateEntry.Date = DateTime.Now;
            CriteriaEntry.Text = string.Empty;
            CriteriaStack.Children.Clear();

            // Add a new criterion entry field
            CriteriaStack.Children.Add(new Label { Text = "Enter Criteria" });
            CriteriaStack.Children.Add(new Entry { WidthRequest = 420, Margin = new Thickness(0, 0, 0, 10), Placeholder = "E.G. Attendance, Participation, etc.", TextColor = Colors.Black });
        }
    }
}