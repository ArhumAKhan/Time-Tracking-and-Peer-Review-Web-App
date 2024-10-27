﻿using System;
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

            // Check if the user has already made a peer review submission
            if (UserHasSubmitted())
            {
                string script = "alert('Error: You have already submitted a peer review for today.');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
                return;
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

            // Enable the submit button and its functionality
            EnableButton(criteria, teamMembers);
        }

        // Function to check if the user has already made a peer review submission
        private bool UserHasSubmitted()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to check if a peer review submission from the user already exists
                string query = "SELECT * FROM pr_submissions " +
                "WHERE submitter_net_id = @submitter_net_id AND submission_date = @submission_date";

                MySqlCommand command = new MySqlCommand(query, connection);

                // Set the parameters for the query
                command.Parameters.AddWithValue("@submitter_net_id", Session["net_id"].ToString());
                command.Parameters.AddWithValue("@submission_date", DateTime.Now.ToString("yyyy-MM-dd"));

                // Execute the query and check if a submission exists
                int count = Convert.ToInt32(command.ExecuteScalar());

                // Close DB connection
                connection.Close();

                // Return true if a submission exists, false otherwise
                if (count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        // Function to get the peer review criteria for the current date
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

        // Function to get the user's team members using their net ID
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

        // Function to build the table for the peer review form
        private void BuildTable(List<Criteria> criteria, List<TeamMembers> teamMembers)
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
                Text = "Member Name"
            };
            headerRow.Cells.Add(headerCell);

            // Insert the criteria as headers for rest of the columns
            foreach (Criteria c in criteria)
            {
                headerCell = new TableHeaderCell
                {
                    Text = c.CriteriaDesc
                };
                headerRow.Cells.Add(headerCell);
            }

            // Add the header row to the table
            table.Rows.Add(headerRow);

            // Add a row for each team member
            foreach (TeamMembers tm in teamMembers)
            {
                // Label first cell with the team member's name
                TableRow row = new TableRow();
                TableCell cell = new TableCell
                {
                    Text = tm.Name
                };
                row.Cells.Add(cell);

                // Add a dropdown list for rating member on each criteria
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

                // Add the row to the table
                table.Rows.Add(row);
            }

            // Add the table to the placeholder
            ReviewTablePlaceholder.Controls.Add(table);
        }

        // Function to enable the submit button for the peer review form and add functionality
        private void EnableButton(List<Criteria> criteria, List<TeamMembers> teamMembers)
        {
            Button submitButton = FindControl("SubmitButton") as Button;
            submitButton.Visible = true;
            submitButton.Click += (s, args) => SubmitButton_Click(criteria, teamMembers);
        }

        // Submission button functionality to insert the peer review ratings into the database
        protected void SubmitButton_Click(List<Criteria> criteria, List<TeamMembers> teamMembers)
        {
            // Get the table
            Table reviewTable = ReviewTablePlaceholder.FindControl("ReviewTable") as Table;

            // Create a list to store the ratings
            var ratingData = new List<Rating>();

            // For each member (row) in the table
            for (int i = 1; i < reviewTable.Rows.Count; i++)
            {
                TableRow row = reviewTable.Rows[i];

                // Get the net ID of the corresponding member
                string for_id = teamMembers[i - 1].NetId;

                // For each criteria (column) in the table
                for (int j = 1; j < row.Cells.Count; j++)
                {
                    TableCell cell = row.Cells[j];

                    // Get the criteria ID
                    int criteria_id = criteria[j - 1].CriteriaId;

                    // Get the dropdown list
                    DropDownList ddl = cell.Controls.OfType<DropDownList>().FirstOrDefault();

                    // Compile the rating data
                    Rating rating = new Rating
                    {
                        Criteria_id = criteria_id,
                        For_net_id = for_id,
                        From_net_id = Session["net_id"].ToString(),
                        RatingValue = int.Parse(ddl.SelectedValue)
                    };

                    // Add the rating to the list
                    ratingData.Add(rating);
                }
            }

            // If there are ratings to insert, insert them into the database
            if (ratingData.Count > 0)
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // For each rating
                    foreach (Rating rating in ratingData)
                    {
                        // Insert the rating into DB
                        string query = "INSERT INTO peer_review_ratings (course_id, criteria_id, for_net_id, from_net_id, rating) " +
                                        "VALUES (@course_id, @criteria_id, @for_net_id, @from_net_id, @rating)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Set the parameters for the query
                            command.Parameters.AddWithValue("@course_id", "CS4485.JC");
                            command.Parameters.AddWithValue("@criteria_id", rating.Criteria_id);
                            command.Parameters.AddWithValue("@for_net_id", rating.For_net_id);
                            command.Parameters.AddWithValue("@from_net_id", rating.From_net_id);
                            command.Parameters.AddWithValue("@rating", rating.RatingValue);

                            command.ExecuteNonQuery();
                        }
                    }

                    string insertion = "INSERT INTO pr_submissions (submission_date, submitter_net_id)" +
                                        "VALUES (@submission_date, @submitter_net_id)";

                    using (MySqlCommand command = new MySqlCommand(insertion, connection))
                    {
                        // Set the parameters for the query
                        command.Parameters.AddWithValue("@submission_date", DateTime.Now.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@submitter_net_id", Session["net_id"].ToString());

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            } else
            {
                // If there are no ratings to insert, display an error message
                string script = "alert('Error: Failed to submit peer review.');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
                return;
            }
        }

        // Class for storing team member data
        public class TeamMembers
        {
            public string Name { get; set; }
            public string NetId { get; set; }
        }

        // Class for storing criteria data
        public class Criteria
        {
            public int CriteriaId { get; set; }
            public string CriteriaDesc { get; set; }
        }

        // Class for storing rating data
        public class Rating
        {
            public int Criteria_id { get; set; }
            public string For_net_id { get; set; }
            public string From_net_id { get; set; }
            public int RatingValue { get; set; }
        }
    }
}