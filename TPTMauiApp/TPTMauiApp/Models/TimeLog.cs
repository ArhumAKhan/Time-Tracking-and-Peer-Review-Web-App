/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TPTMauiApp.Models
{
    [Table("time_logs")]
    public class TimeLog
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [ForeignKey("Student")]
        [Column("student_id")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        [Column("course_id")]
        public int CourseId { get; set; }

        [Column("log_date")]
        public DateTime LogDate { get; set; }

        [Column("hours_logged")]
        public decimal HoursLogged { get; set; }

        // Navigation properties
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}