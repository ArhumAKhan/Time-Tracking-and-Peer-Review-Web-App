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
    public class Student
    {
        [Key]
        [Column("student_id")]
        public int StudentId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("major")]
        public string Major { get; set; }

        // Navigation property
        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}