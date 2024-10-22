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

                BuildTable();
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
                            string name = reader["last_name"].ToString() + ", " + reader["first_name"].ToString();
                            string netId = reader["net_id"].ToString();

                            teamMembers.Add(new TeamMembers { Name = name, NetId = netId });
                        }

                        //string teamMembersJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(teamMembers);
                        //string script = $"<script>console.log({teamMembersJson});</script>";
                        //ClientScript.RegisterStartupScript(this.GetType(), "PrintTeamMembers", script);

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

                        //string criteriaJson = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(criteria);
                        //string script = $"<script>console.log({criteriaJson});</script>";
                        //ClientScript.RegisterStartupScript(this.GetType(), "PrintCriteria", script);

                        connection.Close();
                    }
                }
            }
        }

        private void BuildTable()
        {
            Table table = new Table();
            TableRow headerRow = new TableRow();

            TableHeaderCell headerCell = new TableHeaderCell();
            headerCell.Text = "Member Name";
            headerRow.Cells.Add(headerCell);

            foreach (Criteria c in criteria)
            {
                headerCell = new TableHeaderCell();
                headerCell.Text = c.CriteriaDesc;
                headerRow.Cells.Add(headerCell);
            }

            table.Rows.Add(headerRow);

            foreach (TeamMembers tm in teamMembers)
            {
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                cell.Text = tm.Name;
                row.Cells.Add(cell);

                foreach (Criteria c in criteria)
                {
                    cell = new TableCell();
                    cell.ID = tm.NetId + "_" + c.CriteriaId;
                    cell.CssClass = "rating";

                    DropDownList ddl = new DropDownList();
                    ddl.Items.Add(new ListItem("0", "0"));
                    ddl.Items.Add(new ListItem("1", "1"));
                    ddl.Items.Add(new ListItem("2", "2"));
                    ddl.Items.Add(new ListItem("3", "3"));
                    ddl.Items.Add(new ListItem("4", "4"));
                    ddl.Items.Add(new ListItem("5", "5"));

                    cell.Controls.Add(ddl);
                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }

            ReviewTable.Controls.Add(table);
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
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
    }
}