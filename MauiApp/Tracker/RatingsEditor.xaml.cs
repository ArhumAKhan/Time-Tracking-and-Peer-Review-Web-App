using MySql.Data.MySqlClient;

namespace Tracker
{
    public partial class RatingsEditor : ContentPage
    {
        public RatingsEditor()
        {
            InitializeComponent();
        }

        private void SearchSubmit_Clicked(object sender, EventArgs e)
        {
            SubmissionsStack.Children.Clear();

            string search_id = SubmitterIdEntry.Text;

            using MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString);

            connection.Open();

            string search_query = "SELECT prr.rating_id, prr.criteria_id, prr.for_net_id, prr.rating, prc.criteria_desc " +
                                  "FROM pr_ratings AS prr JOIN pr_criteria AS prc ON prr.criteria_id = prc.criteria_id " +
                                  "WHERE from_net_id = @search_id;";

            using MySqlCommand command = new MySqlCommand(search_query, connection);

            command.Parameters.AddWithValue("@search_id", search_id);

            using MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string pr_id = reader["for_net_id"]?.ToString() ?? string.Empty;
                string rating = reader["rating"]?.ToString() ?? string.Empty;
                string criteria = reader["criteria_desc"]?.ToString() ?? string.Empty;
                int rating_id = int.Parse(reader["rating_id"]?.ToString() ?? string.Empty);
                int criteria_id = int.Parse(reader["criteria_id"]?.ToString() ?? string.Empty);

                Entry ratingEntry = new () { Text = rating, WidthRequest = 50, Margin = new Thickness(0, 0, 0, 0) };

                Button editRating = new() { Text = "Upadate Rating", Margin = new Thickness(0, 0, 0, 0) };
                editRating.Clicked += (s, e) => EditRating(rating_id, criteria_id, int.Parse(ratingEntry.Text));

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

                layout.Add(new Label { Text = "Rating For: " + pr_id, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, 0, 0);
                layout.Add(new Label { Text = "Criteria: " + criteria, TextColor = Colors.Black, Margin = new Thickness(0, 0, 0, 0) }, 1, 0);
                layout.Add(ratingEntry, 2, 0);
                layout.Add(editRating, 3, 0);

                SubmissionsStack.Children.Add(layout);
            }

            connection.Close();
        }

        private void EditRating(int rating_id, int criteria_id, int new_rating)
        {
            using MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString);

            connection.Open();

            string update_query = "UPDATE pr_ratings SET rating = @new_rating WHERE rating_id = @rating_id AND criteria_id = @criteria_id;";

            using MySqlCommand command = new MySqlCommand(update_query, connection);

            command.Parameters.AddWithValue("@new_rating", new_rating);
            command.Parameters.AddWithValue("@rating_id", rating_id);
            command.Parameters.AddWithValue("@criteria_id", criteria_id);

            command.ExecuteNonQuery();

            connection.Close();

            DisplayAlert("Success", "Rating updated successfully!", "OK");
        }
    }
}