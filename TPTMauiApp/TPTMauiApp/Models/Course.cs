/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

namespace TPTMauiApp.Models;

public class Course
{
    public int CourseId { get; set; } // Maps to course_id
    public string CourseName { get; set; } // Maps to course_name

    public int ProfessorId { get; set; } // Maps to professor_id
    public Professor Professor { get; set; }
    public ICollection<TimeLog> TimeLogs { get; set; }
}
