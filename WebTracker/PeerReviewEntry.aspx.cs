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
using static PeerReviewApp.PeerReviewEntry;

namespace PeerReviewApp
{
    public partial class PeerReviewEntry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //Replace with SQL query for team
                var students = new List<Student>
                {
                    new Student { Id = 1, StudentName = "Farhat" },
                    new Student { Id = 2, StudentName = "Nikhil" },
                    new Student { Id = 3, StudentName = "Jhonny" },
                    new Student { Id = 4, StudentName = "Jaden" },
                    new Student { Id = 5, StudentName = "Tahoor" },
                    new Student { Id = 6, StudentName = "Arhum" }
                };

                var studentNames = students.Select(s => s.StudentName).ToList();
                BindGrid(studentNames);
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
    }
}