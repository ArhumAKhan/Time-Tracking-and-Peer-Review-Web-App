/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using TPTMauiApp.Data;
using TPTMauiApp.Models;

namespace TPTMauiApp.Views
{
    public partial class HoursLoggedView : ContentPage
    {
        private readonly ApplicationDbContext _dbContext;

        public ObservableCollection<Course> Classes { get; set; }
        public Course SelectedClass { get; set; }

        public HoursLoggedView(ApplicationDbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;

            // Load the class list (binding to the Picker)
            LoadProfessorClassesAsync();
            BindingContext = this;
        }

        // Get the professor's classes from the database
        private async void LoadProfessorClassesAsync()
        {
            // Assuming the current professor's ID is known, e.g., stored in session or passed in some way
            int professorId = GetCurrentProfessorId();

            // Query database to get professor's classes
            var classes = await _dbContext
                .Courses.Where(c => c.ProfessorId == professorId)
                .ToListAsync();

            Classes = new ObservableCollection<Course>(classes);
        }

        private async void OnClassSelected(object sender, EventArgs e)
        {
            if (SelectedClass != null)
            {
                // Load time log data for the selected class
                var logs = await GetTimeLogsForClass(SelectedClass.CourseId);
                BuildGrid(logs);
            }
        }

        private async Task<Dictionary<Student, List<TimeLog>>> GetTimeLogsForClass(int courseId)
        {
            // Query the database to retrieve time logs for the selected course.
            var timeLogs = await _dbContext
                .TimeLogs.Where(log => log.CourseId == courseId)
                .Include(log => log.Student) // To include student data
                .ToListAsync();

            // Group time logs by student
            var groupedLogs = timeLogs
                .GroupBy(log => log.Student)
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedLogs;
        }

        private void BuildGrid(Dictionary<Student, List<TimeLog>> logs)
        {
            hoursGrid.Children.Clear();
            hoursGrid.RowDefinitions.Clear();
            hoursGrid.ColumnDefinitions.Clear();

            // Get unique dates from logs
            var uniqueDates = logs.SelectMany(l => l.Value)
                .Select(l => l.LogDate)
                .Distinct()
                .OrderBy(date => date)
                .ToList();

            // Define columns: one per date + 1 for cumulative hours
            foreach (var date in uniqueDates)
                hoursGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            hoursGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Cumulative column

            // Define the first row (dates)
            for (int i = 0; i < uniqueDates.Count; i++)
            {
                var label = new Label { Text = uniqueDates[i].ToShortDateString() };
                hoursGrid.Children.Add(label);
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, i);
            }
            var cumulativeLabel = new Label { Text = "Cumulative" };
            hoursGrid.Children.Add(cumulativeLabel);
            Grid.SetRow(cumulativeLabel, 0);
            Grid.SetColumn(cumulativeLabel, uniqueDates.Count);

            // Fill grid with students and hours logged
            int row = 1;
            foreach (var entry in logs)
            {
                var student = entry.Key;
                var timeLogs = entry.Value;

                // Student's name in the first column
                var studentLabel = new Label { Text = $"{student.FirstName} {student.LastName}" };
                hoursGrid.Children.Add(studentLabel);
                Grid.SetRow(studentLabel, row);
                Grid.SetColumn(studentLabel, 0);

                decimal cumulativeHours = 0;
                for (int col = 0; col < uniqueDates.Count; col++)
                {
                    var log = timeLogs.FirstOrDefault(t => t.LogDate == uniqueDates[col]);
                    decimal hoursLogged = log != null ? log.HoursLogged : 0;
                    cumulativeHours += hoursLogged;

                    // Hours logged on each date
                    var hoursLabel = new Label { Text = hoursLogged.ToString("F1") };
                    hoursGrid.Children.Add(hoursLabel);
                    Grid.SetRow(hoursLabel, row);
                    Grid.SetColumn(hoursLabel, col + 1); // Shift column to the right
                }

                // Cumulative hours
                var cumulativeHoursLabel = new Label { Text = cumulativeHours.ToString("F1") };
                hoursGrid.Children.Add(cumulativeHoursLabel);
                Grid.SetRow(cumulativeHoursLabel, row);
                Grid.SetColumn(cumulativeHoursLabel, uniqueDates.Count + 1);

                row++;
            }
        }

        // Dummy method to retrieve professor ID (replace with actual logic)
        private int GetCurrentProfessorId()
        {
            // Replace with actual method to retrieve the current professor's ID (e.g., from session or login)
            return 1;
        }
    }
}
