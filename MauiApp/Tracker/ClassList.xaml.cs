using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Tracker;

// ******************************************************************************
// * Class List Page for Tracker Application
// *
// * Written by Nikhil Giridharan, Johnny An, and Arhum Khan for CS 4485.
// * NetID: nxg220038, hxa210014, axk210013
// *
// * This page displays a list of courses for a specific professor and allows the
// * user to select a course to view attendance logs. The selected course is then passed
// * to the TimeLog page for further details.
// *
// ******************************************************************************

namespace Tracker
{
    public partial class ClassList : ContentPage
    {
        private int _userId; // Professor's user ID
        private Course _selectedCourse; // Selected course object

        public ClassList(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadClassList();
        }

        // ** Course Class **
        // Represents a course with ID, Code, and Name.
        public class Course
        {
            public string CourseId { get; set; }
            public string CourseCode { get; set; }
            public string CourseName { get; set; }

            public string DisplayName => $"{CourseCode}: {CourseName}";
        }

        private void LoadClassList()
        {
            try
            {
                var courseList = new List<Course>();

                using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    string query = @"
                    SELECT c.course_id, c.course_code, c.course_name 
                    FROM courses c
                    JOIN professors p ON c.professor_id = p.professor_id
                    WHERE p.user_id = @userId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courseList.Add(new Course
                                {
                                    CourseId = reader["course_id"].ToString(),
                                    CourseCode = reader["course_code"].ToString(),
                                    CourseName = reader["course_name"].ToString()
                                });
                            }
                        }
                    }
                }

                ClassPicker.ItemsSource = courseList; // Bind list to Picker
                ClassPicker.ItemDisplayBinding = new Binding("DisplayName"); // Bind display to formatted string
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Unable to load class list: " + ex.Message, "OK");
            }
        }

        private void OnClassSelected(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex != -1)
            {
                _selectedCourse = (Course)ClassPicker.SelectedItem;
                TimeLogButton.IsEnabled = true;
            }
            else
            {
                _selectedCourse = null;
                TimeLogButton.IsEnabled = false;
            }
        }

        private async void OnTimeLogButtonClicked(object sender, EventArgs e)
        {
            if (_selectedCourse != null)
            {
                await Navigation.PushAsync(new TimeLog(_selectedCourse.CourseId));
            }
            else
            {
                await DisplayAlert("Error", "Please select a class before proceeding.", "OK");
            }
        }
    }
}
