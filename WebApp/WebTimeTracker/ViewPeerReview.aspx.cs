using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using static PeerReviewApp.PeerReviewEntry;

// NOTE: SOME COMMENTS MAY BE INACCURATE (AS THIS ROUGH DRAFT IS MOSTLY COPY AND PASTE FROM PeerReviewEntry.aspx.cs)

namespace WebTimeTracker
{
    public partial class ViewPeerReview : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Validate that the user is logged in
            if (Session["utd_id"] == null || Session["net_id"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            // Get the current date
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            string user_net_ID = Session["net_id"].ToString();

            List<int> pr_ids = FindPeerReview(currentDate);

            foreach (int pr_id in pr_ids)
            {
                // Get the current date and find any peer review criteria for that date
                List<Criteria> criteria = GetCriteria(pr_id);

                BuildTable(criteria, pr_id, user_net_ID);
            }
        }

        // Function to find the peer review for the current date
        private List<int> FindPeerReview(string currentDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to find the peer review accepting submissions for the current date
                string pr_query = "SELECT pr_id " +
                                  "FROM peer_review " +
                                  "WHERE end_date < @currentDate";

                using (MySqlCommand command = new MySqlCommand(pr_query, connection))
                {
                    // Set the parameters for the query
                    command.Parameters.AddWithValue("@currentDate", "2024-11-05");

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<int> pr_ids = new List<int>();

                        // Compile all found criteria into a list
                        while (reader.Read())
                        {
                            int pr_id = int.Parse(reader["pr_id"].ToString());

                            pr_ids.Add(pr_id);
                        }

                        // Close DB connection and return the list of criteria
                        connection.Close();
                        return pr_ids;
                    }
                }
            }
        }

        // Function to get the peer review criteria for the current date
        private List<Criteria> GetCriteria(int pr_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the criteria for the current date
                string criteria_query = "SELECT criteria_id, criteria_desc " +
                                        "FROM pr_criteria " +
                                        "WHERE pr_id = @pr_id";

                using (MySqlCommand criteria_command = new MySqlCommand(criteria_query, connection))
                {
                    // Set the parameters for the query
                    criteria_command.Parameters.AddWithValue("@pr_id", pr_id);

                    using (MySqlDataReader reader = criteria_command.ExecuteReader())
                    {
                        List<Criteria> criteria = new List<Criteria>();

                        // Compile all found criteria into a list
                        while (reader.Read())
                        {
                            int criteriaId = int.Parse(reader["criteria_id"].ToString());
                            string criteriaDesc = reader["criteria_desc"].ToString();

                            criteria.Add(new Criteria { CriteriaId = criteriaId, CriteriaDesc = criteriaDesc });
                        }

                        // Close DB connection and return the list of criteria
                        connection.Close();
                        return criteria;
                    }
                }
            }
        }

        // Function to build the table for the peer review form
        private void BuildTable(List<Criteria> criteria, int pr_id, string user_net_id)
        {
            // Initialize the table
            Table table = new Table
            {
                ID = "ReviewTable"
            };

            // Create the header row for the table
            TableRow headerRow = new TableRow();

            // Add label for member name column
            TableHeaderCell headerCell = new TableHeaderCell
            {
                Text = ""
            };
            headerRow.Cells.Add(headerCell);

            TableRow tableRatingsRow = new TableRow();

            TableCell avgLabelCell = new TableCell
            {
                Text = "Average Rating:"
            };

            tableRatingsRow.Cells.Add(avgLabelCell);

            // Insert the criteria as headers for rest of the columns
            foreach (Criteria c in criteria)
            {
                headerCell = new TableHeaderCell
                {
                    Text = c.CriteriaDesc
                };
                headerRow.Cells.Add(headerCell);

                double avgRating = GetAverageRating(pr_id, c.CriteriaId, user_net_id);

                TableCell ratingsCell = new TableCell{
                    Text = avgRating.ToString()
                };

                tableRatingsRow.Cells.Add(ratingsCell);
            }

            // Add the header row to the table
            table.Rows.Add(headerRow);

            table.Rows.Add(tableRatingsRow);

            // Add the table to the placeholder
            RatingsTablePlaceholder.Controls.Add(table);
        }

        private double GetAverageRating(int pr_id, int criteria_id, string user_net_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the criteria for the current date
                string criteria_query = "SELECT AVG(prr.rating) " +
                                        "FROM pr_ratings AS prr JOIN peer_review AS pr ON prr.pr_id = pr.pr_id " +
                                        "WHERE prr.for_net_id = @user_net_id AND pr.pr_id = @pr_id";

                using (MySqlCommand rating_command = new MySqlCommand(criteria_query, connection))
                {
                    // Set the parameters for the query
                    rating_command.Parameters.AddWithValue("@user_net_id", user_net_id);
                    rating_command.Parameters.AddWithValue("@pr_id", pr_id);

                    // Execute the query and get the peer review ID
                    object result = rating_command.ExecuteScalar();

                    // Save the peer review ID if it exists
                    double avgRating = result != DBNull.Value ? Convert.ToDouble(result) : -1;

                    System.Diagnostics.Debug.WriteLine("Average rating: " + avgRating);

                    // Close DB connection and return the list of criteria
                    connection.Close();

                    return avgRating;
                }
            }
        }
    }

    // Class for storing criteria data
    public class Criteria
    {
        public int CriteriaId { get; set; }
        public string CriteriaDesc { get; set; }
    }
}