using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using static PeerReviewApp.PeerReviewEntry;
using System.Configuration;

namespace PeerReviewApp
{
    public partial class PeerReviewEntry : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Validate that the user is logged in
            if (Session["utd_id"] == null || Session["net_id"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            // Get the current date and find any peer review criteria for that date
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            List<Criteria> criteria = GetCriteria(currentDate);

            // If there are no criteria for the current date, peer review submission is not allowed
            if (criteria.Count < 1)
            {
                string script = "alert('Error: No peer review submission for today.');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
                return;
            }

            // Get the user's team using their net ID
            string user_net_ID = Session["net_id"].ToString();
            List<TeamMembers> teamMembers = GetTeam(user_net_ID);

            // Build the table for the peer review form using the criteria and team members
            BuildTable(criteria, teamMembers);

            // Open the submit button and add functionality using the criteria and team members
            BuildButton(criteria, teamMembers);
        }

        private List<Criteria> GetCriteria(string review_date)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the criteria for the current date
                string query = "SELECT criteria_id, criteria_desc " +
                                "FROM peer_review_criteria " +
                                "WHERE course_id = @course_id AND review_date = @review_date";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Set the parameters for the query
                    command.Parameters.AddWithValue("@course_id", "CS4485.JC");
                    command.Parameters.AddWithValue("@review_date", review_date);

                    using (MySqlDataReader reader = command.ExecuteReader())
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

        private List<TeamMembers> GetTeam(string user_net_ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the user's team members
                string query = "SELECT u.first_name, u.last_name, u.net_id " +
                                "FROM users u " +
                                "JOIN team_members tm ON u.net_id = tm.member_net_id " +
                                "WHERE tm.team_number = (" +
                                "SELECT team_number " +
                                "FROM team_members " +
                                "WHERE member_net_id = @user_net_ID" +
                                ")";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Set the parameter for the query
                    command.Parameters.AddWithValue("@user_net_ID", user_net_ID);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        List<TeamMembers> teamMembers = new List<TeamMembers>();

                        // Compile all found team members into a list
                        while (reader.Read())
                        {
                            string name = reader["last_name"].ToString() + ", " + reader["first_name"].ToString();
                            string netId = reader["net_id"].ToString();

                            teamMembers.Add(new TeamMembers { Name = name, NetId = netId });
                        }

                        // Close DB connection and return the list of team members
                        connection.Close();
                        return teamMembers;
                    }
                }
            }
        }

        private void BuildTable(List<Criteria> criteria, List<TeamMembers> teamMembers)
        {
            Table table = new Table
            {
                ID = "ReviewTable"
            };
            TableRow headerRow = new TableRow();

            TableHeaderCell headerCell = new TableHeaderCell
            {
                Text = "Member Name"
            };
            headerRow.Cells.Add(headerCell);

            foreach (Criteria c in criteria)
            {
                headerCell = new TableHeaderCell
                {
                    Text = c.CriteriaDesc
                };
                headerRow.Cells.Add(headerCell);
            }

            table.Rows.Add(headerRow);

            foreach (TeamMembers tm in teamMembers)
            {
                TableRow row = new TableRow();
                TableCell cell = new TableCell
                {
                    Text = tm.Name
                };
                row.Cells.Add(cell);

                foreach (Criteria c in criteria)
                {
                    DropDownList ddl = new DropDownList();
                    ddl.Items.Add(new ListItem("0", "0"));
                    ddl.Items.Add(new ListItem("1", "1"));
                    ddl.Items.Add(new ListItem("2", "2"));
                    ddl.Items.Add(new ListItem("3", "3"));
                    ddl.Items.Add(new ListItem("4", "4"));
                    ddl.Items.Add(new ListItem("5", "5"));

                    cell = new TableCell();
                    cell.Controls.Add(ddl);
                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }

            ReviewTablePlaceholder.Controls.Add(table);
        }

        private void BuildButton(List<Criteria> criteria, List<TeamMembers> teamMembers)
        {
            Button submitButton = FindControl("SubmitButton") as Button;
            submitButton.Visible = true;
            submitButton.Click += (s, args) => SubmitButton_Click(criteria, teamMembers);
        }

        protected void SubmitButton_Click(List<Criteria> criteria, List<TeamMembers> teamMembers)
        {
            Table reviewTable = ReviewTablePlaceholder.FindControl("ReviewTable") as Table;
            var ratingData = new List<Rating>();

            for (int i = 1; i < reviewTable.Rows.Count; i++)
            {
                TableRow row = reviewTable.Rows[i];
                string for_id = teamMembers[i - 1].NetId;

                for (int j = 1; j < row.Cells.Count; j++)
                {
                    TableCell cell = row.Cells[j];
                    int criteria_id = criteria[j - 1].CriteriaId;

                    DropDownList ddl = cell.Controls.OfType<DropDownList>().FirstOrDefault();

                    Rating rating = new Rating
                    {
                        Criteria_id = criteria_id,
                        For_net_id = for_id,
                        From_net_id = Session["net_id"].ToString(),
                        RatingValue = int.Parse(ddl.SelectedValue)
                    };
                    ratingData.Add(rating);
                }
            }

            if (ratingData.Count > 0)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (Rating rating in ratingData)
                    {
                        string query = "INSERT INTO peer_review_ratings (course_id, criteria_id, for_net_id, from_net_id, rating) " +
                                       "VALUES (@course_id, @criteria_id, @for_net_id, @from_net_id, @rating)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@course_id", "CS4485.JC");
                            command.Parameters.AddWithValue("@criteria_id", rating.Criteria_id);
                            command.Parameters.AddWithValue("@for_net_id", rating.For_net_id);
                            command.Parameters.AddWithValue("@from_net_id", rating.From_net_id);
                            command.Parameters.AddWithValue("@rating", rating.RatingValue);

                            command.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
            }
        }

        public class TeamMembers
        {
            public string Name { get; set; }
            public string NetId { get; set; }
        }

        public class Criteria
        {
            public int CriteriaId { get; set; }
            public string CriteriaDesc { get; set; }
        }

        public class Rating
        {
            public int Criteria_id { get; set; }
            public string For_net_id { get; set; }
            public string From_net_id { get; set; }
            public int RatingValue { get; set; }
        }
    }
}