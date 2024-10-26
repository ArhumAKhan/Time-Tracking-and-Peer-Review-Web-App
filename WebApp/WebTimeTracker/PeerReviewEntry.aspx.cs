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
        private List<TeamMembers> teamMembers;
        private List<Criteria> criteria;

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["utd_id"] == null || Session["net_id"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            criteria = new List<Criteria>();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            GetCriteria(currentDate);

            if (criteria.Count < 1)
            {
                string script = "alert('Error: No peer review submission for today.');";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
                return;
            }

            teamMembers = new List<TeamMembers>();
            string user_net_ID = Session["net_id"]?.ToString();
            GetTeam(user_net_ID);

            BuildTable();

            Button submitButton = FindControl("SubmitButton") as Button;
            submitButton.Visible = true;
        }

        private void GetTeam(string user_net_ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

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
                    command.Parameters.AddWithValue("@user_net_ID", user_net_ID);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["last_name"].ToString() + ", " + reader["first_name"].ToString();
                            string netId = reader["net_id"].ToString();

                            teamMembers.Add(new TeamMembers { Name = name, NetId = netId });
                        }

                        connection.Close();
                    }
                }
            }
        }

        private void GetCriteria(string review_date)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT criteria_id, criteria_desc " +
                                "FROM peer_review_criteria " +
                                "WHERE course_id = @course_id AND review_date = @review_date";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@course_id", "CS4485.JC");
                    command.Parameters.AddWithValue("@review_date", review_date);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int criteriaId = int.Parse(reader["criteria_id"].ToString());
                            string criteriaDesc = reader["criteria_desc"].ToString();

                            criteria.Add(new Criteria { CriteriaId = criteriaId, CriteriaDesc = criteriaDesc });
                        }

                        connection.Close();
                    }
                }
            }
        }

        private void BuildTable()
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
                    ID = c.CriteriaId.ToString(),
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
                    ID = tm.NetId,
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

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            Table reviewTable = ReviewTablePlaceholder.FindControl("ReviewTable") as Table;

            var ratingData = new List<Rating>();

            for (int i = 1; i < reviewTable.Rows.Count; i++)
            {
                TableRow row = reviewTable.Rows[i];
                string for_id = row.Cells[0].ID;

                for (int j = 1; j < row.Cells.Count; j++)
                {
                    TableCell cell = row.Cells[j];
                    string criteria_id = reviewTable.Rows[0].Cells[j].ID;

                    DropDownList ddl = cell.Controls.OfType<DropDownList>().FirstOrDefault();

                    Rating rating = new Rating
                    {
                        criteria_id = int.Parse(criteria_id),
                        for_net_id = for_id,
                        from_net_id = Session["net_id"].ToString(),
                        rating = int.Parse(ddl.SelectedValue)
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
                            command.Parameters.AddWithValue("@criteria_id", rating.criteria_id);
                            command.Parameters.AddWithValue("@for_net_id", rating.for_net_id);
                            command.Parameters.AddWithValue("@from_net_id", rating.from_net_id);
                            command.Parameters.AddWithValue("@rating", rating.rating);

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
            public int criteria_id { get; set; }
            public string for_net_id { get; set; }
            public string from_net_id { get; set; }
            public int rating { get; set; }
        }
    }
}