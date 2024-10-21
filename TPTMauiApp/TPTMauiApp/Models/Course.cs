/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TPTMauiApp.Models
{
    public class Course
    {
        [Key]
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("course_name")]
        public string CourseName { get; set; }

        [ForeignKey("Professor")]
        [Column("professor_id")]
        public int ProfessorId { get; set; }

        // Navigation properties
        public Professor Professor { get; set; }
        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}
