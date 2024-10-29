using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Tracker;

namespace Tracker
{
    public partial class ClassList : ContentPage
    {
        private int _utdId;
        private string _selectedCourseId;

        public ClassList(int utdId)
        {
            InitializeComponent();
            _utdId = utdId;
            LoadClassList();
        }

        private void LoadClassList()
        {
            try
            {
                var courseList = new List<string>();

                using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
                {
                    connection.Open();

                    // Query to get course IDs for the specific professor
                    string query = "SELECT course_id FROM courses WHERE professor_id = @utdId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@utdId", _utdId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string courseId = reader.GetString("course_id");
                                courseList.Add(courseId);
                            }
                        }
                    }
                }

                // Set the ItemsSource of the Picker
                ClassPicker.ItemsSource = courseList;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Unable to load class list: " + ex.Message, "OK");
            }
        }

        // Event handler for when a class is selected
        private void OnClassSelected(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex != -1)
            {
                _selectedCourseId = ClassPicker.SelectedItem.ToString();
                TimeLogButton.IsEnabled = true; // Enable the button when a course is selected
            }
            else
            {
                _selectedCourseId = null;
                TimeLogButton.IsEnabled = false; // Disable the button if no course is selected
            }
        }

        // Event handler for navigating to the TimeLogPage
        private async void OnTimeLogButtonClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedCourseId))
            {
                // Navigate to TimeLogPage and pass the selected course ID
                await Navigation.PushAsync(new TimeLog(_selectedCourseId));
            }
            else
            {
                await DisplayAlert("Error", "Please select a class before proceeding.", "OK");
            }
        }
    }
}
