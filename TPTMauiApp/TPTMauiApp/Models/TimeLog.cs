/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

namespace TPTMauiApp.Models;

public class TimeLog
{
    public int LogId { get; set; } // Primary key
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal HoursLogged { get; set; }

    public Student Student { get; set; }
    public Course Course { get; set; }
}
