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

namespace PeerReviewApp
{
    public partial class PeerReviewEntry : System.Web.UI.Page
    {
        private List<TeamMembers> teamMembers;
        private List<Criteria> criteria;

        private readonly string connectionString = "server=localhost;user=root;database=team73_db;port=3306;password=team73";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                teamMembers = new List<TeamMembers>();
                criteria = new List<Criteria>();

                // These are placeholders. Replace with actual values from session or page constructor.
                GetTeam("jxd123456");
                GetCriteria("2024-10-21");

                BindGrid(teamMembers.Select(tm => $"{tm.FirstName} {tm.LastName}").ToList());
            }
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
                            string firstName = reader["first_name"].ToString();
                            string lastName = reader["last_name"].ToString();
                            string netId = reader["net_id"].ToString();

                            teamMembers.Add(new TeamMembers { FirstName = firstName, LastName = lastName, NetId = netId });
                        }

                        string teamMembersJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(teamMembers);
                        string script = $"<script>console.log({teamMembersJson});</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "PrintTeamMembers", script);

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
                    command.Parameters.AddWithValue("@course_id", "CS4485");
                    command.Parameters.AddWithValue("@review_date", review_date);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int criteriaId = int.Parse(reader["criteria_id"].ToString());
                            string criteriaDesc = reader["criteria_desc"].ToString();

                            criteria.Add(new Criteria { CriteriaId = criteriaId, CriteriaDesc = criteriaDesc });
                        }

                        string criteriaJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(criteria);
                        string script = $"<script>console.log({criteriaJson});</script>";
                        ClientScript.RegisterStartupScript(this.GetType(), "PrintCriteria", script);

                        connection.Close();
                    }
                }
            }
        }
        private void BindGrid(List<string> studentNames)
        {

            PeerReviewGrid.DataSource = studentNames.Select(name => new { StudentName = name }).ToList();
            PeerReviewGrid.DataBind();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            var students = new List<Student>();

            foreach (GridViewRow row in PeerReviewGrid.Rows)
            {
                var studentName = row.Cells[0].Text;
                var studentId = GetStudentIdByName(studentName);

                var ratings = new List<Rating>
                {
                    new Rating { CriteriaId = "Criteria1", Value = int.Parse(((DropDownList)row.FindControl("Criteria1")).SelectedValue) },
                    new Rating { CriteriaId = "Criteria2", Value = int.Parse(((DropDownList)row.FindControl("Criteria2")).SelectedValue) },
                    new Rating { CriteriaId = "Criteria3", Value = int.Parse(((DropDownList)row.FindControl("Criteria3")).SelectedValue) },
                    new Rating { CriteriaId = "Criteria4", Value = int.Parse(((DropDownList)row.FindControl("Criteria4")).SelectedValue) },
                    new Rating { CriteriaId = "Criteria5", Value = int.Parse(((DropDownList)row.FindControl("Criteria5")).SelectedValue) }
                };

                students.Add(new Student { Id = studentId, Ratings = ratings, StudentName = studentName });
            }

            var json = JsonConvert.SerializeObject(students, Formatting.Indented);
            string script = $"console.log({JsonConvert.SerializeObject(json)});";
            ClientScript.RegisterStartupScript(this.GetType(), "showjson", script, true);
        }

        private int GetStudentIdByName(string studentName)
        {
            var students = new List<Student>
            {
                new Student { Id = 1, StudentName = "Farhat" },
                new Student { Id = 2, StudentName = "Nikhil" },
                new Student { Id = 3, StudentName = "Jhonny" },
                new Student { Id = 4, StudentName = "Jaden" },
                new Student { Id = 5, StudentName = "Tahoor" },
                new Student { Id = 6, StudentName = "Arhum" }
            };

            return students.FirstOrDefault(s => s.StudentName == studentName)?.Id ?? 0;
        }

        public class Student
        {
            public int Id { get; set; }
            public string StudentName { get; set; }
            public List<Rating> Ratings { get; set; }
        }

        public class Rating
        {
            public string CriteriaId { get; set; }
            public int Value { get; set; }
        }

        public class TeamMembers
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NetId { get; set; }
        }

        public class Criteria
        {
            public int CriteriaId { get; set; }
            public string CriteriaDesc { get; set; }
        }
    }
}