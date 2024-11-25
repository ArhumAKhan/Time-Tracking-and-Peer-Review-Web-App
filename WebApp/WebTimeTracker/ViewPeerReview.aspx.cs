using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using static WebTimeTracker.PeerReviewEntry;

namespace WebTimeTracker
{
    // ******************************************************************************
    // * Page for Viewing PR Ratings for WebTracker Application
    // *
    // * Written by Farhat R. Kabir for CS 4485.
    // * NetID: frk200000
    // *
    // * This page allows a logged-in user to view the ratings they have received for
    // * a closed peer review. The page searches for peer reviews that have ended and
    // * displays the average rating for each criteria in each peer review.
    // * 
    // ******************************************************************************
    public partial class ViewPeerReview : System.Web.UI.Page
    {
        // ** DB Connection String **
        // This string is used to connect to the MySQL database using the connection string from Web.config.
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        // ** Page Load Event **
        // This method is called every time the page is loaded or refreshed.
        // It validates user login, checks for an closed peer reviews, and
        // shows average ratings per criteria for any found peer reviews.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                // Validate that the user is logged in.
                if (Session["student_id"] == null || Session["net_id"] == null)
                {
                    Response.Redirect("Login.aspx");
                }

                // Get the current date.
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                // Get the user's net ID from the session.
                string user_net_ID = Session["net_id"].ToString();

                // Find closed PRs.
                List<int> pr_ids = FindPeerReview(currentDate);

                // Retrieve average ratings per criteria for each PR.
                for (int i = 0; i < pr_ids.Count; i++)
                {
                    int pr_id = pr_ids[i];

                    // Find criteria for PR.
                    List<Criteria> criteria = GetCriteria(pr_id);

                    // Build average ratings table.
                    BuildTable(criteria, pr_id, user_net_ID);
                }
            }
        }

        // ** Method For Finding Closed Peer Reviews **
        // This method is called upon page load after user validation with the current date.
        // It queries the database to find closed PRs, and returns their ids.
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
                    command.Parameters.AddWithValue("@currentDate", currentDate);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<int> pr_ids = new List<int>();

                        // Compile all PR ids into a list
                        while (reader.Read())
                        {
                            int pr_id = int.Parse(reader["pr_id"].ToString());

                            pr_ids.Add(pr_id);
                        }

                        // Close DB connection and return the list of PR ids.
                        connection.Close();
                        return pr_ids;
                    }
                }
            }
        }

        // ** Method For Finding PR Criteria **
        // This method is called for every closed PR found.
        // It queries the database to find criteria of given PR and returns their IDs as a list;
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

        // ** Method For Building Average Ratings Table **
        // This method is called with information for a PR.
        // It uses given data to query the database and list average rating per criteria for a PR
        private void BuildTable(List<Criteria> criteria, int pr_id, string user_net_id)
        {
            // Initialize the table
            Table table = new Table
            {
                ID = "ReviewTable"
            };

            // Create the header row for the table.
            TableRow headerRow = new TableRow();

            // Empty top right corner cell.
            TableHeaderCell headerCell = new TableHeaderCell
            {
                Text = ""
            };
            headerRow.Cells.Add(headerCell);

            TableRow tableRatingsRow = new TableRow();

            // Label for ratings row
            TableCell avgLabelCell = new TableCell
            {
                Text = "Average Rating:"
            };
            tableRatingsRow.Cells.Add(avgLabelCell);

            // Insert the criteria and average rating into the table
            foreach (Criteria c in criteria)
            {
                // Insert criteria
                headerCell = new TableHeaderCell
                {
                    Text = c.CriteriaDesc
                };
                headerRow.Cells.Add(headerCell);

                // Find average rating for the crieteria. -1 if no rating found.
                double avgRating = GetAverageRating(pr_id, c.CriteriaId, user_net_id);

                // Display average rating, or "Pending" if not found.
                TableCell ratingsCell = new TableCell{
                    Text = avgRating != -1 ? avgRating.ToString() : "Pending"
                };
                tableRatingsRow.Cells.Add(ratingsCell);
            }

            // Add rows to table.
            table.Rows.Add(headerRow);
            table.Rows.Add(tableRatingsRow);

            // Add the table to the placeholder.
            RatingsTablePlaceholder.Controls.Add(table);
        }

        // ** Method For Getting Average Rating For Criteria **
        // This method is called with information for a PR.
        // Queries database for average rating a user recieved for a criteria
        private double GetAverageRating(int pr_id, int criteria_id, string user_net_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the criteria for the current date
                string criteria_query = "SELECT AVG(prr.rating) " +
                                        "FROM pr_ratings AS prr JOIN peer_review AS pr ON prr.pr_id = pr.pr_id " +
                                        "WHERE prr.for_net_id = @user_net_id AND pr.pr_id = @pr_id AND criteria_id = @criteria_id";

                using (MySqlCommand rating_command = new MySqlCommand(criteria_query, connection))
                {
                    // Set the parameters for the query
                    rating_command.Parameters.AddWithValue("@user_net_id", user_net_id);
                    rating_command.Parameters.AddWithValue("@pr_id", pr_id);
                    rating_command.Parameters.AddWithValue("@criteria_id", criteria_id);

                    // Execute the query
                    object result = rating_command.ExecuteScalar();

                    // Grab average rating (or -1 if none found)
                    double avgRating = result != DBNull.Value ? Convert.ToDouble(result) : -1;

                    // Close DB connection and return average rating
                    connection.Close();
                    return avgRating;
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
}