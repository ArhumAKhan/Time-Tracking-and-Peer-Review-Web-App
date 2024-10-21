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
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("netID")]
        public string NetID { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("role")]
        public string Role { get; set; }  // Enum: 'student', 'professor', 'admin'
    }
}