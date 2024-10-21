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
    public class Professor
    {
        [Key]
        [Column("professor_id")]
        public int ProfessorId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("department")]
        public string Department { get; set; }

        // Navigation property
        public ICollection<Course> Courses { get; set; }
    }
}
