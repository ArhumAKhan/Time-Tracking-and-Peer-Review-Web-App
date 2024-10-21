/*
  Author: Nikhil Giridharan
  NetID: nxg220038
  Date: 9.22.2024
  Class: CS 4485.0W1
  Assignment: Time Tracking Desktop App (Part 2)

  C# file defines the ViewModel for the MainPage in the time tracking app.
  ViewModel handles the logic of displaying the professor's classes, managing the student's logged time,
  calculating total weekly hours, modifying time entries and emailing students.
*/

using System.Collections.ObjectModel; // For binding lists of data to the UI.
using System.ComponentModel;
using System.Windows.Input;

namespace TPTMauiApp;

// MainPageViewModel is the ViewModel that supports data binding to the MainPage UI.
// It holds the list of professor's classes, student time data, and commands to modify
// time entries and send emails to students.
public class MainPageViewModel : INotifyPropertyChanged
{
    // Collection of professor's classes
    public ObservableCollection<string> ProfessorClasses { get; set; }

    // Hold the currently selected class.
    public string SelectedClass { get; set; }

    // Collection of student time data
    public ObservableCollection<StudentHours> StudentsHours { get; set; }

    // Hold the total hours logged by all students for the current week.
    public int TotalWeekHours { get; set; }

    // Command for modifying a student's time entry.
    public ICommand ModifyTimeCommand { get; }

    // Command for emailing students who need to log hours
    public ICommand EmailStudentsCommand { get; }

    // Initializes demo data
    public MainPageViewModel()
    {
        //Demo list of professor's classes
        ProfessorClasses = new ObservableCollection<string>
        {
            "CS4485 - Fall 2024",
            "CS3345 - Spring 2024",
        };

        // Demo list of students' logged hours
        StudentsHours = new ObservableCollection<StudentHours>
        {
            new StudentHours
            {
                StudentName = "John Doe",
                Day1Hours = "02:00",
                Day2Hours = "01:30",
                TotalHours = "03:30",
            },
            new StudentHours
            {
                StudentName = "Jane Smith",
                Day1Hours = "04:00",
                Day2Hours = "02:00",
                TotalHours = "06:00",
            },
        };

        // Demo total hours for week.
        TotalWeekHours = 9;

        // Commands for modifying time and emailing students.
        ModifyTimeCommand = new Command(ModifyTime);
        EmailStudentsCommand = new Command(EmailStudents);
    }

    // Method to handle modifying time entries (not complete).
    void ModifyTime()
    {
        // Placeholder for logic to modify a student's time entry.
    }

    // Method to handle emailing students (not complete)
    void EmailStudents()
    {
        // Send email notifications to students.
    }

    // INotifyPropertyChanged implementation.
    // Used to notify the UI when a bound property changes.
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

// Class representing each student's logged hours for the week. Bind individual student data to the UI.
public class StudentHours
{
    public string StudentName { get; set; } // The student's name.
    public string Day1Hours { get; set; } // Hours logged on the first day of tracking.
    public string Day2Hours { get; set; } // Hours logged on the second day of tracking.
    public string TotalHours { get; set; } // Total hours logged by the student for the week.
}
