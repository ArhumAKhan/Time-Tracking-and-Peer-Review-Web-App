using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using static PeerReviewApp.PeerReviewEntry;

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

            // Get all peer reviews for the user in the current semester
            List<int> pr_ids = FindPeerReviews();

            string user_net_ID = Session["net_id"].ToString();

            // Loop through each peer review and build a table
            foreach (int pr_id in pr_ids)
            {
                List<Criteria> criteria = GetCriteria(pr_id);
                BuildTable(criteria, pr_id, user_net_ID);
            }
        }

        // Function to find all peer reviews for the current semester
        private List<int> FindPeerReviews()
        {
            List<int> pr_ids = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Adjust this query as needed to fit your semester criteria
                string pr_query = "SELECT pr_id " +
                                  "FROM peer_review " +
                                  "WHERE start_date <= @currentDate AND end_date >= @currentDate";

                MySqlCommand pr_command = new MySqlCommand(pr_query, connection);
                pr_command.Parameters.AddWithValue("@currentDate", DateTime.Now.ToString("yyyy-MM-dd"));

                using (MySqlDataReader reader = pr_command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pr_ids.Add(Convert.ToInt32(reader["pr_id"]));
                    }
                }

                connection.Close();
            }

            return pr_ids;
        }

        // Function to get criteria for a specific peer review
        private List<Criteria> GetCriteria(int pr_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string criteria_query = "SELECT criteria_id, criteria_desc " +
                                        "FROM pr_criteria " +
                                        "WHERE pr_id = @pr_id";

                using (MySqlCommand criteria_command = new MySqlCommand(criteria_query, connection))
                {
                    criteria_command.Parameters.AddWithValue("@pr_id", pr_id);

                    List<Criteria> criteria = new List<Criteria>();

                    using (MySqlDataReader reader = criteria_command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            criteria.Add(new Criteria
                            {
                                CriteriaId = Convert.ToInt32(reader["criteria_id"]),
                                CriteriaDesc = reader["criteria_desc"].ToString()
                            });
                        }
                    }

                    connection.Close();
                    return criteria;
                }
            }
        }

        // Build a table for each peer review's criteria and ratings
        private void BuildTable(List<Criteria> criteria, int pr_id, string user_net_id)
        {
            Table table = new Table { ID = $"ReviewTable_{pr_id}" };
            TableRow headerRow = new TableRow();

            TableHeaderCell headerCell = new TableHeaderCell { Text = "Criteria" };
            headerRow.Cells.Add(headerCell);

            TableRow tableRatingsRow = new TableRow();
            TableCell avgLabelCell = new TableCell { Text = "Average Rating:" };
            tableRatingsRow.Cells.Add(avgLabelCell);

            foreach (Criteria c in criteria)
            {
                headerCell = new TableHeaderCell { Text = c.CriteriaDesc };
                headerRow.Cells.Add(headerCell);

                double avgRating = GetAverageRating(pr_id, c.CriteriaId, user_net_id);
                TableCell ratingsCell = new TableCell { Text = avgRating.ToString() };
                tableRatingsRow.Cells.Add(ratingsCell);
            }

            table.Rows.Add(headerRow);
            table.Rows.Add(tableRatingsRow);

            RatingsTablePlaceholder.Controls.Add(table);
        }

        private double GetAverageRating(int pr_id, int criteria_id, string user_net_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string criteria_query = "SELECT AVG(prr.rating) " +
                                        "FROM pr_ratings AS prr JOIN peer_review AS pr ON prr.pr_id = pr.pr_id " +
                                        "WHERE prr.for_net_id = @user_net_id AND pr.pr_id = @pr_id AND prr.criteria_id = @criteria_id";

                using (MySqlCommand rating_command = new MySqlCommand(criteria_query, connection))
                {
                    rating_command.Parameters.AddWithValue("@user_net_id", user_net_id);
                    rating_command.Parameters.AddWithValue("@pr_id", pr_id);
                    rating_command.Parameters.AddWithValue("@criteria_id", criteria_id);

                    object result = rating_command.ExecuteScalar();
                    double avgRating = result != null ? Convert.ToDouble(result) : -1;

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
