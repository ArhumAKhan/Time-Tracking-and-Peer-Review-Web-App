/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */


// Models/Professor.cs
namespace TPTMauiApp.Models;

public class Professor
{
    public int ProfessorId { get; set; }  // Maps to professor_id
    public string FirstName { get; set; } // Maps to first_name
    public string LastName { get; set; }  // Maps to last_name
    public string Email { get; set; }     // Maps to email
    public string Department { get; set; } // Maps to department
    public List<Course> Courses { get; set; }
}
