using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Tracker;

namespace Tracker
{
    // ******************************************************************************
    // * Class List Page for Tracker Application
    // *
    // * Written by Nikhil Giridharan and Johnny An for CS 4485.
    // * NetID: nxg220038 and hxa210014
    // *
    // * This page displays a list of courses for a specific professor and allows the
    // * user to select a course to view attendance logs. The selected course is then passed
    // * to the TimeLog page for further details.
    // *
    // ******************************************************************************
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
        // Represents a course with ID and Code.
        public class Course
        {
            public string CourseId { get; set; }
            public string CourseCode { get; set; }
        }

        private void LoadClassList()
        {
            try
            {
                var courseList = new List<Course>();

                using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();
                    // ** SQL Query to Retrieve Courses **
                    // Retrieves course ID and codes for classes taught by the specified professor.
                    string query = "SELECT course_id, course_code FROM courses c " +
                                   "JOIN professors p ON c.professor_id = p.professor_id " +
                                   "WHERE p.user_id = @userId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", _userId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                courseList.Add(new Course
                                {
                                    CourseId = reader["course_id"].ToString(), // Convert Int32 to string
                                    CourseCode = reader["course_code"].ToString()
                                });
                            }
                        }
                    }
                }

                ClassPicker.ItemsSource = courseList; // Bind list to Picker
            }
            catch (Exception ex)
            {
                // Display an error message if class list fails to load.
                DisplayAlert("Error", "Unable to load class list: " + ex.Message, "OK");
            }
        }

        // ** On Class Selected Event **
        // Event handler triggered when the user selects a class from the picker.
        private void OnClassSelected(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex != -1)
            {
                // Get the selected course and enable the TimeLog button
                _selectedCourse = (Course)ClassPicker.SelectedItem;
                TimeLogButton.IsEnabled = true;
            }
            else
            {
                _selectedCourse = null;
                TimeLogButton.IsEnabled = false;
            }
        }

        // ** On Time Log Button Clicked Event **
        // Navigates to the TimeLog page with the selected course ID if a class is selected.
        private async void OnTimeLogButtonClicked(object sender, EventArgs e)
        {
            if (_selectedCourse != null)
            {
                // Pass the selected course ID to the TimeLog page
                await Navigation.PushAsync(new TimeLog(_selectedCourse.CourseId));
            }
            else
            {
                await DisplayAlert("Error", "Please select a class before proceeding.", "OK");
            }
        }
    }
}
