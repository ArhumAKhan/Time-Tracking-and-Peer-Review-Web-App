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
using static WebTimeTracker.PeerReviewEntry;
using System.Configuration;

namespace WebTimeTracker
{
    // ******************************************************************************
    // * Page for Submitting Peer Reviews for WebTracker Application
    // *
    // * Written by Farhat R. Kabir for CS 4485.
    // * NetID: frk200000
    // *
    // * This page allows a logged-in user to submit peer reviews for their team members.
    // * The page loads criteria for an open peer review (if available) and generates a
    // * form for the user to rate each team member on each criterion.
    // * 
    // ******************************************************************************
    public partial class PeerReviewEntry : System.Web.UI.Page
    {
        // ** DB Connection String **
        // This string is used to connect to the MySQL database using the connection string from Web.config.
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        // ** Page Load Event **
        // This method is called every time the page is loaded or refreshed.
        // It validates user login, checks for an open peer review, and generates the peer review form.
        // Once the form is built, it enables the submission functionality.
        // On postback, it hides the form and submit button after submission.
        protected void Page_Load(object sender, EventArgs e)
        {
            // Validate that the user is logged in.
            if (Session["student_id"] == null || Session["net_id"] == null || Session["course_id"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            int courseId = int.Parse(Session["course_id"].ToString());

            // Get the current date.
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            // Check to see if there are any peer reviews for the current date.
            int pr_id = FindPeerReview(currentDate);

            // If there are no peer reviews for the current date, peer review submission is not allowed.
            if (pr_id == -1)
            {
                Literal messageLiteral = new Literal
                {
                    Text = $"<div>No Peer Review is currently expecting submissions.</div>" +
                           "<div>If this sounds incorrect, please contact support.</div>"
                };
                ReviewTablePlaceholder.Controls.Add(messageLiteral);
                return;
            }

            // Get the user's net ID from the session.
            int studentId = int.Parse(Session["student_id"].ToString());

            // Check if the user has already made a submission for this peer review. If so, submission is not allowed.
            if (HasAlreadySubmitted(studentId, pr_id))
            {
                Literal messageLiteral = new Literal
                {
                    Text = $"<div>Congratulations! You've already made a submission for the currently active Peer Review.</div>" +
                           "<div>If this sounds incorrect, please contact support.</div>"
                };
                ReviewTablePlaceholder.Controls.Add(messageLiteral);
                return;
            }

            // Get the current date and find any peer review criteria for that date
            List<Criteria> criteria = GetCriteria(pr_id);

            // Get the user's team using their net ID
            List<TeamMembers> teamMembers = GetTeam(studentId);

            // Build the table for the peer review form using the criteria and team members
            BuildTable(criteria, teamMembers);

            string user_net_ID = Session["net_id"].ToString();

            // Enable the submit button and its functionality
            EnableButton(criteria, teamMembers, user_net_ID, pr_id);
    
            // If the page is a postback (after submission), hide the form and submit button to prevent resubmission
            if(IsPostBack)
            {
                Button submitButton = FindControl("SubmitButton") as Button;
                submitButton.Visible = false;

                Table prTable = FindControl("ReviewTable") as Table;
                prTable.Visible = false;
            }
        }

        // ** Method For Finding Open Peer Review **
        // This method is called upon page load after user validation with the current date.
        // It queries the database to find an open peer review that is accepting submissions for the current date.
        // If an open peer review is found, it returns the peer review ID. Otherwise, it returns -1.
        private int FindPeerReview(string currentDate)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to find the peer review accepting submissions for the current date
                string pr_query = "SELECT pr_id " +
                                  "FROM peer_review " +
                                  "WHERE @review_date BETWEEN start_date AND end_date";

                MySqlCommand pr_command = new MySqlCommand(pr_query, connection);

                // Set the parameter for the query
                pr_command.Parameters.AddWithValue("@review_date", currentDate);

                // Execute the query and get the peer review ID
                object result = pr_command.ExecuteScalar();

                // Save the peer review ID if it exists
                int pr_id = result != null ? Convert.ToInt32(result) : -1;

                // Close DB connection
                connection.Close();

                // Return the peer review ID
                return pr_id;
            }
        }

        // ** Method For Checking Whether User Has Already Made A Submission **
        // This method is called upon page load after an open peer review is found.
        // It queries the database to find if the user has already made a submission for the current peer review.
        // If a submission is found, it returns true. Otherwise, it returns false.
        private bool HasAlreadySubmitted(int studentId, int pr_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to check if a peer review submission from the user already exists
                string query = "SELECT * FROM pr_submissions " +
                               "WHERE submitter_id = @submitter_id AND pr_id = @pr_id";


                MySqlCommand pr_submission_command = new MySqlCommand(query, connection);

                // Set the parameters for the query
                pr_submission_command.Parameters.AddWithValue("@submitter_id", studentId);
                pr_submission_command.Parameters.AddWithValue("@pr_id", pr_id);

                // Execute the query and check if a submission exists
                int count = Convert.ToInt32(pr_submission_command.ExecuteScalar());

                // Close DB connection
                connection.Close();

                // Return true if a submission exists, false otherwise
                return count > 0;
            }
        }

        // ** Method For Grabbing Criteria For Open Peer Review **
        // This method is called upon page load after an open peer review is found and user submission is allowed.
        // It queries the database and returns any criteria associated with the current peer review.
        private List<Criteria> GetCriteria(int pr_id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the criteria for the current peer review
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

        // ** Method For Grabbing Retrieving User's Team **
        // This method is called after an open peer review is found and user submission is allowed.
        // It queries the database and returns a list of the user's team members.
        private List<TeamMembers> GetTeam(int studentId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Query DB to get the user's team members
                string query = "SELECT u.first_name, u.last_name, u.net_id " +
                                "FROM users AS u JOIN students AS stu ON u.user_id = stu.user_id, " +
                                "team_members as tm " +
                                "WHERE tm.student_id = stu.student_id AND tm.team_number = (" +
                                "SELECT team_number " +
                                "FROM team_members " +
                                "WHERE student_id = @student_id" +
                                ")";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Set the parameter for the query
                    command.Parameters.AddWithValue("@student_id", studentId);

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

        // ** Method For Building Peer Review Form **
        // This method is called near end of page load after an open peer review is found, user submission is allowed,
        // and criteria and team members are retrieved. It generates a table with rows for each team member and columns
        // for each criterion, with dropdown lists for the user to rate each team member on each criterion.
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

        // ** Method For Enabling Peer Review Submission **
        // This method is called after the peer review form is built.
        // It allocates all retrieved user and Peer Review data to the submission function and assigns it to the submit button.
        // It also enables the submit button to be visible and clickable, with a confirmation dialog.
        private void EnableButton(List<Criteria> criteria, List<TeamMembers> teamMembers, string user_net_id, int pr_id)
        {
            Button submitButton = FindControl("SubmitButton") as Button;
            submitButton.Visible = true;
            submitButton.OnClientClick = "return confirm('Are you sure you want to submit your peer review?');";
            submitButton.Click += (s, args) => SubmitButton_Click(criteria, teamMembers, user_net_id, pr_id);
        }

        // ** Submit Button Click Event **
        // This method is called when the Submit button is clicked and the user confirms the submission.
        // It retrieves the ratings from the form, inserts them into the database with their associated criteria and target,
        // and registers the submission along with the ratings. If successful, it reloads the page.
        protected void SubmitButton_Click(List<Criteria> criteria, List<TeamMembers> teamMembers, string user_net_id, int pr_id)
        {
            // Get the table
            Table reviewTable = ReviewTablePlaceholder.FindControl("ReviewTable") as Table;

            // Create a list to store the ratings
            var ratingData = new List<Rating>();

            // For each member (row) in the table\
            // i = 1 to skip the header row
            for (int i = 1; i < reviewTable.Rows.Count; i++)
            {
                TableRow row = reviewTable.Rows[i];

                // Get the net ID of the corresponding member
                // (i-1) is used to account for the header row
                string for_id = teamMembers[i - 1].NetId;

                // For each criteria (column) in the table
                // j = 1 to skip the name column
                for (int j = 1; j < row.Cells.Count; j++)
                {
                    TableCell cell = row.Cells[j];

                    // Get the criteria ID
                    // (j-1) is used to account for the name column
                    int criteria_id = criteria[j - 1].CriteriaId;

                    // Get the dropdown list
                    DropDownList ddl = cell.Controls.OfType<DropDownList>().FirstOrDefault();

                    // Compile the rating data
                    Rating rating = new Rating
                    {
                        Criteria_id = criteria_id,
                        For_net_id = for_id,
                        From_net_id = user_net_id,
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

                    // Register the submission in the pr_submissions table
                    string pr_submission_insert = "INSERT INTO pr_submissions (pr_id, submission_date, submitter_net_id) " +
                                                  "VALUES (@pr_id, @submission_date, @submitter_net_id)";

                    MySqlCommand pr_submission_command = new MySqlCommand(pr_submission_insert, connection);

                    pr_submission_command.Parameters.AddWithValue("@pr_id", pr_id);
                    pr_submission_command.Parameters.AddWithValue("@submission_date", DateTime.Now.ToString("yyyy-MM-dd"));
                    pr_submission_command.Parameters.AddWithValue("@submitter_net_id", user_net_id);

                    pr_submission_command.ExecuteNonQuery();

                    // Get the submission ID
                    int submission_id = (int)pr_submission_command.LastInsertedId;

                    // For each rating
                    foreach (Rating rating in ratingData)
                    {
                        // Insert the rating into DB while linking it to the submission
                        string query = "INSERT INTO pr_ratings (submission_id, pr_id, criteria_id, from_net_id, for_net_id, rating) " +
                                       "VALUES (@submission_id, @pr_id, @criteria_id, @from_net_id, @for_net_id, @rating)";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            // Set the parameters for the query
                            command.Parameters.AddWithValue("@submission_id", submission_id);
                            command.Parameters.AddWithValue("@pr_id", pr_id);
                            command.Parameters.AddWithValue("@criteria_id", rating.Criteria_id);
                            command.Parameters.AddWithValue("@from_net_id", rating.From_net_id);
                            command.Parameters.AddWithValue("@for_net_id", rating.For_net_id);
                            command.Parameters.AddWithValue("@rating", rating.RatingValue);

                            command.ExecuteNonQuery();
                        }
                    }

                    // Close the DB connection and reload the page
                    connection.Close();
                    Response.Redirect("PeerReviewEntry.aspx");
                }
            }
            else
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