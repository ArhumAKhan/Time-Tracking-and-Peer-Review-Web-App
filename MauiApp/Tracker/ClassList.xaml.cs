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
    // * This page displays a list of classes for a specific professor and allows the
    // * user to select a class to view attendance logs. The selected class is then passed
    // * to the TimeLog page for further details.
    // *
    // ******************************************************************************

    public partial class ClassList : ContentPage
    {
        private int _utdId; // Professor's UTD ID
        private string _selectedCourseId; // Selected course ID

        // ** Constructor **
        // Initializes the ClassList page with the professor's UTD ID and loads the class list.
        public ClassList(int utdId)
        {
            InitializeComponent();
            _utdId = utdId;
            LoadClassList();
        }

        // ** Load Class List **
        // Connects to the database to retrieve a list of course IDs associated with the professor's UTD ID.
        private void LoadClassList()
        {
            try
            {
                var courseList = new List<string>();

                using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    // ** SQL Query to Retrieve Courses **
                    // Retrieves course IDs for classes taught by the specified professor.
                    string query = "SELECT course_id FROM courses WHERE professor_id = @utdId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@utdId", _utdId);

                        using (var reader = command.ExecuteReader())
                        {
                            // Add each course ID to the course list
                            while (reader.Read())
                            {
                                string courseId = reader.GetString("course_id");
                                courseList.Add(courseId);
                            }
                        }
                    }
                }

                // Set the ItemsSource of the Picker to the list of course IDs
                ClassPicker.ItemsSource = courseList;
            }
            catch (Exception ex)
            {
                // ** Error Handling **
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
                // Store the selected course ID and enable the TimeLog button
                _selectedCourseId = ClassPicker.SelectedItem.ToString();
                TimeLogButton.IsEnabled = true;
            }
            else
            {
                // Clear the selected course ID and disable the TimeLog button if no class is selected
                _selectedCourseId = null;
                TimeLogButton.IsEnabled = false;
            }
        }

        // ** On Time Log Button Clicked Event **
        // Navigates to the TimeLog page with the selected course ID if a class is selected.
        private async void OnTimeLogButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedCourseId))
            {
                // ** Navigation to TimeLog Page **
                // Pass the selected course ID to the TimeLog page.
                await Navigation.PushAsync(new TimeLog(_selectedCourseId));
            }
            else
            {
                // Display an error if no class is selected before proceeding
                await DisplayAlert("Error", "Please select a class before proceeding.", "OK");
            }
        }
    }
}
