using MySql.Data.MySqlClient;

namespace Tracker
{
    // ******************************************************************************
    // * Ratings Editor Page for Tracker Application
    // *
    // * Written by Farhat R. Kabir for CS 4485.
    // * NetID: frk200000
    // *
    // * This page allows professors to edit rating values in submitted Peer Review Forms
    // * The user can search for submissions by selecting a student, and then edit any
    // * rating values (which are unique to a criteria and the member the rating is for)
    // *
    // ******************************************************************************

    public partial class RatingsEditor : ContentPage
    {
        private readonly int userId;
        private readonly int courseId;

        // ** Constructor **
        // Initializes the ratings page and retrieves user's ID and course ID
        // and loads list of students
        public RatingsEditor(int user_id)
        {
            InitializeComponent();
            this.userId = user_id;

            using MySqlConnection connection = new(DatabaseConfig.ConnectionString);

            connection.Open();

            string courseQuery = "SELECT c.course_id " +
                                  "FROM courses AS c, users AS u JOIN professors AS prf ON u.user_id = prf.user_id " +
                                  "WHERE u.user_id = @user_id AND c.professor_id = prf.professor_id";
            using MySqlCommand courseSearchCommand = new(courseQuery, connection);
            courseSearchCommand.Parameters.AddWithValue("@user_id", userId);
            courseId = (int)courseSearchCommand.ExecuteScalar();

            string studentQuery = "SELECT u.net_id, u.first_name, u.last_name " +
                                  "FROM users AS u JOIN students AS stu ON u.user_id = stu.user_id " +
                                  "JOIN team_members AS tm ON tm.student_id = stu.student_id " +
                                  "WHERE tm.course_id = @course_id";
            using MySqlCommand studentSearchCommand = new(studentQuery, connection);
            studentSearchCommand.Parameters.AddWithValue("@course_id", courseId);

            using MySqlDataReader reader = studentSearchCommand.ExecuteReader();

            // For each student in the course, add their Net ID to the search field
            while (reader.Read()) 
            {
                string net_id = reader["net_id"]?.ToString() ?? string.Empty;
                string first_name = reader["first_name"]?.ToString() ?? string.Empty;
                string last_name = reader["last_name"]?.ToString() ?? string.Empty;

                StudentPicker.Items.Add(net_id + " - " + first_name + " " + last_name);
            }

            connection.Close();
        }

        // ** Student Selected Event **
        // This method is called when the user selects a student. The method takes the
        // Net ID value from the selected option and queries DB for all ratings submitted by
        // the user. For every rating, it displays the criteria, rating value, and an option to
        // update the rating.
        private void OnStudentSelected(object sender, EventArgs e)
        {
            // Clear the existing ratings from the stack layout
            SubmissionsStack.Children.Clear();

            if(StudentPicker.SelectedIndex == -1)
            {
                return;
            }

            // Get the Net ID value from the selected option
            string selectedStudent = StudentPicker.Items[StudentPicker.SelectedIndex];
            string search_id = selectedStudent.Split(" - ")[0];

            using MySqlConnection connection = new (DatabaseConfig.ConnectionString);

            connection.Open();

            string studentQuery = "SELECT student_id FROM students AS stu JOIN users AS u ON stu.user_id = u.user_id WHERE u.net_id = @search_id";
            using MySqlCommand studentSearchCommand = new (studentQuery, connection);
            studentSearchCommand.Parameters.AddWithValue("@search_id", search_id);
            int studentId = (int)studentSearchCommand.ExecuteScalar();

            //// SQL for querying all ratings submitted by the user and the associated criteria
            string ratingsSearch = "SELECT prr.rating_id, prr.criteria_id, u.first_name, u.last_name, prr.rating, prc.criteria_desc " +
                                   "FROM pr_ratings AS prr JOIN pr_criteria AS prc ON prr.criteria_id = prc.criteria_id " +
                                   "JOIN peer_review AS pr ON prr.pr_id = pr.pr_id, " +
                                   "users AS u JOIN students AS stu on u.user_id = stu.user_id " +
                                   "WHERE prr.from_student_id = @student_id AND pr.course_id = @course_id AND stu.student_id = prr.for_student_id";
            using MySqlCommand ratingsSearchCommand = new(ratingsSearch, connection);
            ratingsSearchCommand.Parameters.AddWithValue("@student_id", studentId);
            ratingsSearchCommand.Parameters.AddWithValue("@course_id", courseId);

            using MySqlDataReader reader = ratingsSearchCommand.ExecuteReader();

            // For each rating, display the criteria, rating value, and an option to update the rating
            while (reader.Read())
            {
                string firstName = reader["first_name"]?.ToString() ?? string.Empty;
                string lastName = reader["last_name"]?.ToString() ?? string.Empty;
                string rating = reader["rating"]?.ToString() ?? string.Empty;
                string criteria = reader["criteria_desc"]?.ToString() ?? string.Empty;
                int rating_id = int.Parse(reader["rating_id"]?.ToString() ?? string.Empty);
                int criteria_id = int.Parse(reader["criteria_id"]?.ToString() ?? string.Empty);

                // Entry field containing the rating value that can be updated
                Entry ratingEntry = new () { Text = rating, WidthRequest = 50, Margin = new Thickness(0, 0, 0, 0) };

                // Button to update the rating value
                Button editRating = new() { Text = "Upadate Rating", Margin = new Thickness(0, 0, 0, 0) };
                editRating.Clicked += (s, e) => EditRating(rating_id, criteria_id, int.Parse(ratingEntry.Text));

                // Grid layout to display the rating information
                Grid layout = new ()
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Auto }
                    }
                };

                // Add the rating data, rating value input field, and update button to the layout
                layout.Add(new Frame { Content = new Label { Text = "Rating For: " + firstName + " " + lastName, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, BorderColor = Colors.Gray, Padding = 5, WidthRequest = 150, HeightRequest = 50 }, 0, 0);
                layout.Add(new Frame { Content = new Label { Text = "Criteria: " + criteria, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, BorderColor = Colors.Gray, Padding = 5, WidthRequest = 150, HeightRequest = 50 }, 1, 0);
                layout.Add(new Frame { Content = ratingEntry, BorderColor = Colors.Gray, Padding = 5, WidthRequest = 150, HeightRequest = 50 }, 2, 0);
                layout.Add(new Frame { Content = editRating, BorderColor = Colors.Gray, Padding = 5, WidthRequest = 150, HeightRequest = 50 }, 3, 0);

                // Add the layout to the ratings stack for the page
                SubmissionsStack.Children.Add(layout);
            }

            connection.Close();
        }

        // ** Edit Rating Clicked Event **
        // Update rating buttons are dynamically assigned this method to update the rating value
        // for the specific rating and criteria. Upon clicking the button, the rating value is
        // updated in the DB.
        private void EditRating(int rating_id, int criteria_id, int new_rating)
        {
            using MySqlConnection connection = new (DatabaseConfig.ConnectionString);

            connection.Open();

            string update_query = "UPDATE pr_ratings SET rating = @new_rating WHERE rating_id = @rating_id AND criteria_id = @criteria_id;";

            using MySqlCommand command = new (update_query, connection);

            command.Parameters.AddWithValue("@new_rating", new_rating);
            command.Parameters.AddWithValue("@rating_id", rating_id);
            command.Parameters.AddWithValue("@criteria_id", criteria_id);

            command.ExecuteNonQuery();

            connection.Close();

            // Display a success message upon updating the rating
            DisplayAlert("Success", "Rating updated successfully!", "OK");
        }
    }
}