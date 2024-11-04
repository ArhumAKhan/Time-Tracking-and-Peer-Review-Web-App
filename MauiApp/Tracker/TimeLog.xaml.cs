using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Maui.Controls;

namespace Tracker
{
    public partial class TimeLog : ContentPage
    {
        public string ClassName { get; private set; }
        private List<DateTime> DateHeaders { get; set; }
        private List<StudentAttendanceRecord> AttendanceRecords { get; set; }

        public TimeLog(string className)
        {
            ClassName = className;
            InitializeComponent();
            DateHeaders = new List<DateTime>();
            AttendanceRecords = new List<StudentAttendanceRecord>();
            LoadAttendanceLog();
        }
        private void LoadAttendanceLog()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

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

                            while (reader.Read())
                            {
                                int utdId = reader.GetInt32("utd_id");
                                DateTime logDate = reader.GetDateTime("log_date");
                                int hoursLogged = reader.GetInt32("hours_logged");
                                int minutesLogged = reader.GetInt32("minutes_logged");
                                string studentName = reader.GetString("student_name");

                                // Track unique dates for the header
                                uniqueDates.Add(logDate);

                                // Add the log to the corresponding student's list
                                if (!attendanceData.ContainsKey(utdId))
                                {
                                    attendanceData[utdId] = (studentName, new List<DailyLog>());
                                }
                                attendanceData[utdId].logs.Add(new DailyLog { Date = logDate, Hours = hoursLogged, Minutes = minutesLogged });
                            }

                            // Sort the dates and add to DateHeaders
                            DateHeaders = uniqueDates.OrderBy(d => d).ToList();

                            // Process each student's data and calculate cumulative hours
                            foreach (var studentLog in attendanceData)
                            {
                                int studentId = studentLog.Key;
                                var studentName = studentLog.Value.studentName;
                                var logs = studentLog.Value.logs;

                                // Initialize cumulative hours and minutes
                                int totalHours = 0;
                                int totalMinutes = 0;

                                // Create a dictionary to store hours worked for each date
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

                                // Create a list of daily hours matching the sorted date headers
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

                                // Add to the AttendanceRecords collection
                                AttendanceRecords.Add(new StudentAttendanceRecord
                                {
                                    StudentId = studentId,
                                    StudentName = studentName, // Use the studentName from the query
                                    CumulativeHours = formattedCumulativeHours,
                                    DailyHours = dailyHoursList
                                });
                            }
                        }
                    }

                    // Generate the table-like Grid layout
                    GenerateGridLayout();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the database operation
                    DisplayAlert("Error", "Unable to load attendance data: " + ex.Message, "OK");
                }
            }
        }

        private void GenerateGridLayout()
        {
            // Clear existing children in the grid
            AttendanceGrid.Children.Clear();
            AttendanceGrid.ColumnDefinitions.Clear();
            AttendanceGrid.RowDefinitions.Clear();

            // Add column definitions for student name, cumulative hours, and each date
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); // Student name
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); // Cumulative hours
            foreach (var date in DateHeaders)
            {
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Add row definitions for header and each student
            AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Header row
            foreach (var _ in AttendanceRecords)
            {
                AttendanceGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Add header labels with borders
            AddCellWithBorder("Student Name", 0, 0, FontAttributes.Bold);
            AddCellWithBorder("Cumulative Hours", 0, 1, FontAttributes.Bold);
            for (int i = 0; i < DateHeaders.Count; i++)
            {
                AddCellWithBorder(DateHeaders[i].ToString("MM/dd/yyyy"), 0, i + 2, FontAttributes.Bold);
            }

            // Add student data rows with borders
            for (int row = 0; row < AttendanceRecords.Count; row++)
            {
                var record = AttendanceRecords[row];

                AddCellWithBorder(record.StudentName, row + 1, 0);
                AddCellWithBorder(record.CumulativeHours, row + 1, 1);

                // Add daily hours data with borders
                for (int col = 0; col < record.DailyHours.Count; col++)
                {
                    AddCellWithBorder(record.DailyHours[col], row + 1, col + 2);
                }
            }
        }

        // Helper method to add a cell with a border
        private void AddCellWithBorder(string text, int row, int column, FontAttributes fontAttributes = FontAttributes.None)
        {
            // Create the label
            var label = new Label
            {
                Text = text,
                FontAttributes = fontAttributes,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(5)
            };

            // Create a Frame to wrap the label and simulate a border
            var frame = new Frame
            {
                Content = label,
                BorderColor = Colors.Gray,
                CornerRadius = 0, // No rounded corners
                Padding = 0, // No padding inside the frame
                HasShadow = false // Disable shadow to make it look like a border
            };

            // Add the frame to the main AttendanceGrid
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