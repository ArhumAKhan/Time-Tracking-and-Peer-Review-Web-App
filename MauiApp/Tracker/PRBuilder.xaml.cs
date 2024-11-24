using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;

namespace Tracker
{
    public partial class PRBuilder : ContentPage
    {
        public PRBuilder()
        {
            InitializeComponent();
        }

        private void AddCriterion_Clicked(object sender, EventArgs e)
        {
            // Create a new Label and Entry for the criteria
            var newLabel = new Label
            {
                Text = "Enter Criterion",
                TextColor = Colors.Black,
                Margin = new Thickness(20, 0, 0, 00)
            };

            var newEntry = new Entry
            {
                WidthRequest = 420,
                Margin = new Thickness(0, 0, 0, 10),
                Placeholder = "E.G. Attendance, Participation, etc.",
                TextColor = Colors.Black
            };

            // Add the new Label and Entry to the StackLayout
            CriteriaStack.Children.Add(newLabel);
            CriteriaStack.Children.Add(newEntry);
        }

        private void RemoveCriterion_Clicked(object sender, EventArgs e)
        {
            if (CriteriaStack.Children.Count > 2)
            {
                CriteriaStack.Children.RemoveAt(CriteriaStack.Children.Count - 1);
                CriteriaStack.Children.RemoveAt(CriteriaStack.Children.Count - 1);
            }
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            string startDate = StartDateEntry.Date.ToString("yyyy-MM-dd");
            string endDate = EndDateEntry.Date.ToString("yyyy-MM-dd");

            List<string> criteria_list = new List<string>();

            foreach (var child in CriteriaStack.Children)
            {
                if (child is Entry entry)
                {
                    System.Diagnostics.Debug.WriteLine(entry.Text);
                    criteria_list.Add(entry.Text);
                }
            }

            using var connection = new MySqlConnection(DatabaseConfig.ConnectionString);

            connection.Open();

            string pr_insert = "INSERT INTO peer_review (start_date, end_date) VALUES (@startDate, @endDate);";

            using var command = new MySqlCommand(pr_insert, connection);
            
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);

            command.ExecuteNonQuery();

            int pr_id = (int)command.LastInsertedId;
            System.Diagnostics.Debug.WriteLine("Last Inserted ID: " + pr_id);

            foreach (string criteria in criteria_list)
            {
                string criteria_insert = "INSERT INTO pr_criteria (pr_id, criteria_desc) VALUES (@pr_id, @criteria);";

                using var criteria_command = new MySqlCommand(criteria_insert, connection);

                criteria_command.Parameters.AddWithValue("@pr_id", pr_id);
                criteria_command.Parameters.AddWithValue("@criteria", criteria);

                criteria_command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}