using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;

namespace Tracker
{
    // ******************************************************************************
    // * Time Log Page for Tracker Application
    // *
    // * Written by Nikhil Giridharan and Johnny An for CS 4485.
    // * NetID: nxg220038 and hxa210014
    // *
    // * This page retrieves and displays time logs for a specific class in a table format.
    // * Each row represents a student's attendance log with daily hours and cumulative hours.
    // *
    // ******************************************************************************

    public partial class TimeLog : ContentPage
    {
        public string ClassName { get; private set; }
        private List<DateTime> DateHeaders { get; set; }
        private List<StudentAttendanceRecord> AttendanceRecords { get; set; }

        // ** Constructor **
        // Initializes the time log page with the specified class name and loads attendance data.
        public TimeLog(string className)
        {
            ClassName = className;
            InitializeComponent();
            DateHeaders = new List<DateTime>();
            AttendanceRecords = new List<StudentAttendanceRecord>();
            LoadAttendanceLog();
        }

        // ** Load Attendance Log **
        // Connects to the database and retrieves attendance records for each student in the class.
        // Records are organized by date and displayed in a grid format with cumulative hours.
        private void LoadAttendanceLog()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // ** SQL Query to Retrieve Attendance Data **
                    // Retrieves UTD ID, log date, hours logged, minutes logged, and student name for the specified class.
                    string query = @"SELECT tl.utd_id, log_date, hours_logged, minutes_logged, CONCAT(first_name, ' ', last_name) AS student_name
                                     FROM time_logs tl
                                     JOIN users us ON tl.utd_id = us.utd_id
                                     WHERE course_id = @courseId
                                     ORDER BY log_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@courseId", ClassName);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            var attendanceData = new Dictionary<int, (string studentName, List<DailyLog> logs)>();
                            var uniqueDates = new HashSet<DateTime>();

                            // ** Process Data Rows **
                            // Read each row and organize data by student, tracking unique dates for the header.
                            while (reader.Read())
                            {
                                int utdId = reader.GetInt32("utd_id");
                                DateTime logDate = reader.GetDateTime("log_date");
                                int hoursLogged = reader.GetInt32("hours_logged");
                                int minutesLogged = reader.GetInt32("minutes_logged");
                                string studentName = reader.GetString("student_name");

                                uniqueDates.Add(logDate);

                                if (!attendanceData.ContainsKey(utdId))
                                {
                                    attendanceData[utdId] = (studentName, new List<DailyLog>());
                                }
                                attendanceData[utdId].logs.Add(new DailyLog { Date = logDate, Hours = hoursLogged, Minutes = minutesLogged });
                            }

                            // Sort unique dates and store in DateHeaders
                            DateHeaders = uniqueDates.OrderBy(d => d).ToList();

                            // Process each student's data to calculate cumulative hours
                            foreach (var studentLog in attendanceData)
                            {
                                int studentId = studentLog.Key;
                                var studentName = studentLog.Value.studentName;
                                var logs = studentLog.Value.logs;

                                int totalHours = 0;
                                int totalMinutes = 0;

                                // Calculate hours worked per date and cumulative hours
                                var hoursPerDate = new Dictionary<DateTime, (int hours, int minutes)>();
                                foreach (var log in logs)
                                {
                                    totalHours += log.Hours;
                                    totalMinutes += log.Minutes;
                                    hoursPerDate[log.Date] = (log.Hours, log.Minutes);
                                }

                                // Convert totalMinutes to hours if 60 or more
                                totalHours += totalMinutes / 60;
                                totalMinutes = totalMinutes % 60;

                                // Create a list of daily hours matching sorted dates
                                var dailyHoursList = new List<string>();
                                foreach (var date in DateHeaders)
                                {
                                    if (hoursPerDate.ContainsKey(date))
                                    {
                                        var (hours, minutes) = hoursPerDate[date];
                                        dailyHoursList.Add($"{hours:D2}:{minutes:D2}");
                                    }
                                    else
                                    {
                                        dailyHoursList.Add("00:00");
                                    }
                                }

                                // Format cumulative hours as HH:MM
                                string formattedCumulativeHours = $"{totalHours:D2}:{totalMinutes:D2}";

                                // Add student attendance record to the collection
                                AttendanceRecords.Add(new StudentAttendanceRecord
                                {
                                    StudentId = studentId,
                                    StudentName = studentName,
                                    CumulativeHours = formattedCumulativeHours,
                                    DailyHours = dailyHoursList
                                });
                            }
                        }
                    }

                    // Generate grid layout to display attendance records
                    GenerateGridLayout();
                }
                catch (Exception ex)
                {
                    // ** Error Handling **
                    // Display an error message if attendance data fails to load.
                    DisplayAlert("Error", "Unable to load attendance data: " + ex.Message, "OK");
                }
            }
        }

        // ** Generate Grid Layout **
        // Dynamically creates a grid layout for attendance records with student names, cumulative hours, and daily logs.
        private void GenerateGridLayout()
        {
            // Clear existing children and define grid columns and rows
            AttendanceGrid.Children.Clear();
            AttendanceGrid.ColumnDefinitions.Clear();
            AttendanceGrid.RowDefinitions.Clear();

            // Add column definitions for student name, cumulative hours, and each date
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            foreach (var date in DateHeaders)
            {
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Define rows for header and each student
            AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            foreach (var _ in AttendanceRecords)
            {
                AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Add header labels
            AddCellWithBorder("Student Name", 0, 0, FontAttributes.Bold);
            AddCellWithBorder("Cumulative Hours", 0, 1, FontAttributes.Bold);
            for (int i = 0; i < DateHeaders.Count; i++)
            {
                AddCellWithBorder(DateHeaders[i].ToString("MM/dd/yyyy"), 0, i + 2, FontAttributes.Bold);
            }

            // Add student data rows
            for (int row = 0; row < AttendanceRecords.Count; row++)
            {
                var record = AttendanceRecords[row];

                AddCellWithBorder(record.StudentName, row + 1, 0);
                AddCellWithBorder(record.CumulativeHours, row + 1, 1);

                for (int col = 0; col < record.DailyHours.Count; col++)
                {
                    AddCellWithBorder(record.DailyHours[col], row + 1, col + 2);
                }
            }
        }

        // ** Helper Method: Add Cell with Border **
        // Adds a label inside a frame to the grid to simulate a bordered cell.
        private void AddCellWithBorder(string text, int row, int column, FontAttributes fontAttributes = FontAttributes.None)
        {
            var label = new Label
            {
                Text = text,
                FontAttributes = fontAttributes,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(5)
            };

            var frame = new Frame
            {
                Content = label,
                BorderColor = Colors.Gray,
                CornerRadius = 0,
                Padding = 0,
                HasShadow = false
            };

            AttendanceGrid.Children.Add(frame);
            Grid.SetRow(frame, row);
            Grid.SetColumn(frame, column);
        }
    }

    public class DailyLog
    {
        public DateTime Date { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    public class StudentAttendanceRecord
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string CumulativeHours { get; set; }
        public List<string> DailyHours { get; set; }
    }
}
