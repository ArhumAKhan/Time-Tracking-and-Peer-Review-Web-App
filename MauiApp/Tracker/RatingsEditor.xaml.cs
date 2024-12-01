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
    // * The user can search for submissions by the submitter's Net ID, and then edit any
    // * rating values (which are unique to a criteria and the member the rating is for)
    // *
    // ******************************************************************************

    public partial class RatingsEditor : ContentPage
    {
        private readonly int userId;

        // ** Constructor **
        // Initializes the login page and its components.
        public RatingsEditor(int user_id)
        {
            InitializeComponent();
            this.userId = user_id;
        }

        // ** Search Button Clicked Event **
        // This method is called when the user clicks the Search button. The search button takes
        // the Net ID value in the search field and queries DB for all ratings submitted by
        // the user. For every rating, it displays the criteria, rating value, and an option to
        // update the rating.
        private void SearchSubmit_Clicked(object sender, EventArgs e)
        {
            // Clear the existing ratings from the stack layout
            SubmissionsStack.Children.Clear();

            // Get the Net ID value from the search field
            string search_id = SubmitterIdEntry.Text;

            using MySqlConnection connection = new (DatabaseConfig.ConnectionString);

            connection.Open();

            // SQL for querying all ratings submitted by the user and the associated criteria
            string search_query = "SELECT prr.rating_id, prr.criteria_id, prr.for_student_id, prr.rating, prc.criteria_desc " +
                                  "FROM pr_ratings AS prr JOIN pr_criteria AS prc ON prr.criteria_id = prc.criteria_id " +
                                  "JOIN peer_review AS pr ON prr.pr_id = pr.pr_id " +
                                  "WHERE prr.from_student_id = (" +
                                  "SELECT student_id FROM students AS stu JOIN users AS u ON stu.user_id = u.user_id " +
                                  "WHERE u.net_id = @search_id) " +
                                  "AND pr.course_id = (" +
                                  "SELECT c.course_id " + 
                                  "FROM courses AS c, users AS u JOIN professors AS prf ON u.user_id = prf.user_id " +
                                  "WHERE u.user_id = @user_id AND c.professor_id = prf.professor_id)";

            using MySqlCommand command = new (search_query, connection);

            command.Parameters.AddWithValue("@search_id", search_id);
            command.Parameters.AddWithValue("@user_id", userId);

            using MySqlDataReader reader = command.ExecuteReader();

            // For each rating, display the criteria, rating value, and an option to update the rating
            while (reader.Read())
            {
                string pr_id = reader["for_student_id"]?.ToString() ?? string.Empty;
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
                layout.Add(new Label { Text = "Rating For: " + pr_id, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, 0, 0);
                layout.Add(new Label { Text = "Criteria: " + criteria, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, 1, 0);
                layout.Add(ratingEntry, 2, 0);
                layout.Add(editRating, 3, 0);

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