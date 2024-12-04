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
        private Dictionary<(int row, int column), Entry> entryDictionary;
        private bool isEditing = false;



        public TimeLog(string className)
        {
            ClassName = className;
            InitializeComponent();
            DateHeaders = new List<DateTime>();
            AttendanceRecords = new List<StudentAttendanceRecord>();
            entryDictionary = new Dictionary<(int row, int column), Entry>();
            LoadAttendanceLog();

            // Add the Edit button to the toolbar
            var editToolbarItem = new ToolbarItem
            {
                Text = "Edit",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            editToolbarItem.Clicked += OnEditButtonClicked;
            ToolbarItems.Add(editToolbarItem);

            // Add the Add Student button to the toolbar
            var addStudentToolbarItem = new ToolbarItem
            {
                Text = "Add Student",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            addStudentToolbarItem.Clicked += OnAddStudentButtonClicked;
            ToolbarItems.Add(addStudentToolbarItem);

            // Add the Add date button to the toolbar
            var addDateToolbarItem = new ToolbarItem
            {
                Text = "Add Date",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            addDateToolbarItem.Clicked += OnAddDateButtonClicked;
            ToolbarItems.Add(addDateToolbarItem);
        }


        private void LoadAttendanceLog()
        {
            AttendanceRecords.Clear(); // Clear existing records to avoid duplication
            DateHeaders.Clear(); // Clear existing headers to avoid duplication

            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT tl.student_id, tl.log_date, tl.minutes_logged, tl.course_id, 
                            CONCAT(us.first_name, ' ', us.last_name) AS student_name
                        FROM time_logs tl
                        JOIN students st ON tl.student_id = st.student_id
                        JOIN users us ON st.user_id = us.user_id
                        WHERE tl.course_id = @courseId
                        ORDER BY tl.log_date";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@courseId", int.Parse(ClassName));

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            var attendanceData = new Dictionary<int, (string studentName, int courseId, List<DailyLog> logs)>();
                            var uniqueDates = new HashSet<DateTime>();

                            while (reader.Read())
                            {
                                int studentId = reader.GetInt32("student_id");
                                DateTime logDate = reader.GetDateTime("log_date");
                                int minutesLogged = reader.GetInt32("minutes_logged");
                                int courseId = reader.GetInt32("course_id"); // Retrieve course_id
                                string studentName = reader.GetString("student_name");

                                // If minutes_logged is zero, delete this record from the database
                                if (minutesLogged == 0)
                                {
                                    DeleteZeroMinuteEntry(studentId, logDate);
                                    continue; // Skip processing this record
                                }

                                // Track unique dates for the header
                                uniqueDates.Add(logDate);

                                // Add the log to the corresponding student's list
                                if (!attendanceData.ContainsKey(studentId))
                                {
                                    attendanceData[studentId] = (studentName, courseId, new List<DailyLog>());
                                }

                                attendanceData[studentId].logs.Add(new DailyLog
                                {
                                    Date = logDate,
                                    Hours = minutesLogged / 60,
                                    Minutes = minutesLogged % 60
                                });
                            }

                            // Sort the dates and add to DateHeaders
                            DateHeaders = uniqueDates.OrderBy(d => d).ToList();

                            // Process each student's data and calculate cumulative hours
                            foreach (var studentLog in attendanceData)
                            {
                                int studentId = studentLog.Key;
                                var studentName = studentLog.Value.studentName;
                                int courseId = studentLog.Value.courseId;
                                var logs = studentLog.Value.logs;

                                // Initialize cumulative hours and minutes
                                int totalMinutes = logs.Sum(log => log.Hours * 60 + log.Minutes);

                                // Convert totalMinutes to hours and minutes
                                int totalHours = totalMinutes / 60;
                                totalMinutes = totalMinutes % 60;

                                // Create a dictionary to store hours worked for each date
                                var hoursPerDate = logs.ToDictionary(log => log.Date, log => $"{log.Hours:D2}:{log.Minutes:D2}");

                                // Create a list of daily hours matching the sorted date headers
                                var dailyHoursList = DateHeaders
                                    .Select(date => hoursPerDate.ContainsKey(date) ? hoursPerDate[date] : "00:00")
                                    .ToList();

                                // Format cumulative hours as HH:MM
                                string formattedCumulativeHours = $"{totalHours:D2}:{totalMinutes:D2}";

                                // Add to the AttendanceRecords collection
                                AttendanceRecords.Add(new StudentAttendanceRecord
                                {
                                    StudentId = studentId,
                                    StudentName = studentName,
                                    CumulativeHours = formattedCumulativeHours,
                                    DailyHours = dailyHoursList,
                                    courseId = courseId 
                                });
                            }
                        }
                    }

                    // Generate the table-like Grid layout with fresh data
                    GenerateGridLayout();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the database operation
                    DisplayAlert("Error", "Unable to load attendance data: " + ex.Message, "OK");
                }
            }
        }


        private void DeleteZeroMinuteEntry(int studentId, DateTime logDate)
{
    using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
    {
        connection.Open();

        string deleteQuery = @"
            DELETE FROM time_logs 
            WHERE student_id = @studentId 
            AND log_date = @logDate 
            AND minutes_logged = 0";

        using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection))
        {
            deleteCommand.Parameters.AddWithValue("@studentId", studentId);
            deleteCommand.Parameters.AddWithValue("@logDate", logDate);

            deleteCommand.ExecuteNonQuery();
        }
    }

    // Remove the corresponding log from AttendanceRecords and update DateHeaders
    var recordToRemove = AttendanceRecords.FirstOrDefault(r => r.StudentId == studentId);
    if (recordToRemove != null)
    {
        // Find and remove the log date from DailyHours
        int dateIndex = DateHeaders.IndexOf(logDate);
        if (dateIndex >= 0)
        {
            recordToRemove.DailyHours[dateIndex] = "00:00"; // Set to "00:00" to indicate no hours

            // Remove date from DateHeaders if no students have hours logged
            bool isDateEmpty = AttendanceRecords.All(r => r.DailyHours[dateIndex] == "00:00");
            if (isDateEmpty)
                DateHeaders.RemoveAt(dateIndex);
        }

        // Remove the record if all hours are zero
        bool allZero = recordToRemove.DailyHours.All(h => h == "00:00");
        if (allZero)
            AttendanceRecords.Remove(recordToRemove);
    }
}





        private void GenerateGridLayout()
        {
            // Clear existing children in the grid
            AttendanceGrid.Children.Clear();
            AttendanceGrid.ColumnDefinitions.Clear();
            AttendanceGrid.RowDefinitions.Clear();

            // Add column definitions for student name, cumulative hours, each date, and delete button (conditionally)
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); // Student name
            AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star }); // Cumulative hours
            foreach (var date in DateHeaders)
            {
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }
            if (isEditing)
            {
                AttendanceGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Delete button
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
            if (isEditing)
            {
                AddCellWithBorder("Actions", 0, DateHeaders.Count + 2, FontAttributes.Bold); // Header for delete button column
            }

            // Add student data rows with borders and delete button conditionally
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

                // Conditionally add the delete button if in editing mode
                if (isEditing)
                {
                    var deleteButton = new Button
                    {
                        Text = "Delete",
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CommandParameter = record.StudentId // Pass the StudentId as parameter
                    };
                    deleteButton.Clicked += OnDeleteButtonClicked;

                    AttendanceGrid.Children.Add(deleteButton);
                    Grid.SetRow(deleteButton, row + 1);
                    Grid.SetColumn(deleteButton, DateHeaders.Count + 2);
                }
            }
        }

        // Helper method to add a cell with a border
        private void AddCellWithBorder(string text, int row, int column, FontAttributes fontAttributes = FontAttributes.None)
        {
            // Allow editing only for time columns, which start from the third column onward (index 2)
            bool isTimeColumn = column > 1; // Columns > 1 are time columns
            bool isTimeRow = row > 0;

            if (isEditing && isTimeColumn && isTimeRow)
            {
                var entry = new Entry
                {
                    Text = text,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = fontAttributes,
                    //Padding = new Thickness(5),
                    Keyboard = Keyboard.Text // Allows the user to enter formatted text like HH:MM
                };

                // Store the entry in the dictionary for future reference
                entryDictionary[(row, column)] = entry;

                // Add the entry to the grid with a frame around it
                var frame = new Frame
                {
                    Content = entry,
                    BorderColor = Colors.Gray,
                    CornerRadius = 0,
                    Padding = 0,
                    HasShadow = false
                };

                AttendanceGrid.Children.Add(frame);
                Grid.SetRow(frame, row);
                Grid.SetColumn(frame, column);
            }
            else
            {
                // Use a label for non-editable fields (date headers, student name, cumulative hours)
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
        //Make edit button, and logic for editing.
        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            isEditing = true;

            // Clear existing toolbar items
            ToolbarItems.Clear();

            // Add Submit button
            var submitToolbarItem = new ToolbarItem
            {
                Text = "Submit",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            submitToolbarItem.Clicked += OnSubmitButtonClicked;
            ToolbarItems.Add(submitToolbarItem);

            // Add Cancel button
            var cancelToolbarItem = new ToolbarItem
            {
                Text = "Cancel",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            cancelToolbarItem.Clicked += OnCancelButtonClicked;
            ToolbarItems.Add(cancelToolbarItem);

            // Add Add Student button
            var addStudentToolbarItem = new ToolbarItem
            {
                Text = "Add Student",
                Order = ToolbarItemOrder.Primary,
                Priority = 2
            };
            addStudentToolbarItem.Clicked += OnAddStudentButtonClicked;
            ToolbarItems.Add(addStudentToolbarItem);

            // Add Add Date button
            var addDateToolbarItem = new ToolbarItem
            {
                Text = "Add Date",
                Order = ToolbarItemOrder.Primary,
                Priority = 3
            };
            addDateToolbarItem.Clicked += OnAddDateButtonClicked;
            ToolbarItems.Add(addDateToolbarItem);



            // Refresh the grid layout
            GenerateGridLayout();
        }


        private async void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            // Ensure the data structures are synchronized
            if (AttendanceRecords.Count == 0 || entryDictionary.Count == 0)
            {
                await DisplayAlert("Error", "No data available to submit. Please make changes or reload the data.", "OK");
                return;
            }

            // First, validate and check for changes
            bool isValid = true;
            bool hasChanges = false;
            var updates = new List<TimeLogUpdate>();

            foreach (var kvp in entryDictionary)
            {
                var (row, column) = kvp.Key;

                // Validate row exists in AttendanceRecords
                if (row - 1 >= AttendanceRecords.Count || row < 1)
                {
                    continue; // Skip invalid or removed rows
                }

                var entry = kvp.Value;

                // Validate the time entry
                if (!TimeSpan.TryParse(entry.Text, out TimeSpan time))
                {
                    isValid = false;
                    entry.TextColor = Colors.Red; // Highlight invalid entry
                }
                else
                {
                    var record = AttendanceRecords[row - 1];
                    var date = DateHeaders[column - 2];
                    int totalMinutes = (int)time.TotalMinutes;

                    // Check if the value has changed
                    var existingValue = record.DailyHours[column - 2];
                    string formattedTime = $"{time.Hours:D2}:{time.Minutes:D2}";
                    if (existingValue != formattedTime)
                    {
                        hasChanges = true; // Mark as having changes
                    }

                    updates.Add(new TimeLogUpdate
                    {
                        StudentId = record.StudentId,
                        Date = date,
                        totalMinutes = totalMinutes,
                        courseId = record.courseId,
                        WorkDescription = record.WorkDescription
                    });
                }
            }

            // Handle no changes scenario
            if (!hasChanges)
            {
                await DisplayAlert("Error", "No changes have been made.", "OK");
                return; // Stop further execution
            }

            // Handle invalid entries scenario
            if (!isValid)
            {
                await DisplayAlert("Error", "Invalid time entries detected. Please correct them before submitting.", "OK");
                return; // Stop further execution
            }

            // Save the changes if valid and changes are detected
            SaveUpdatesToDatabase(updates);

            // Exit editing mode
            isEditing = false;

            // Rebuild toolbar
            ToolbarItems.Clear();

            var editToolbarItem = new ToolbarItem
            {
                Text = "Edit",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            editToolbarItem.Clicked += OnEditButtonClicked;
            ToolbarItems.Add(editToolbarItem);

            var addStudentToolbarItem = new ToolbarItem
            {
                Text = "Add Student",
                Order = ToolbarItemOrder.Primary,
                Priority = 2
            };
            addStudentToolbarItem.Clicked += OnAddStudentButtonClicked;
            ToolbarItems.Add(addStudentToolbarItem);

            var addDateToolbarItem = new ToolbarItem
            {
                Text = "Add Date",
                Order = ToolbarItemOrder.Primary,
                Priority = 3
            };
            addDateToolbarItem.Clicked += OnAddDateButtonClicked;
            ToolbarItems.Add(addDateToolbarItem);

            // Reload attendance log to update UI
            AttendanceRecords.Clear();
            LoadAttendanceLog();
        }





        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            isEditing = false;

            // Clear the toolbar and rebuild it
            ToolbarItems.Clear();

            // Add Edit button
            var editToolbarItem = new ToolbarItem
            {
                Text = "Edit",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            editToolbarItem.Clicked += OnEditButtonClicked;
            ToolbarItems.Add(editToolbarItem);

            // Re-add "Add Student" and "Add Date" buttons
            var addStudentToolbarItem = new ToolbarItem
            {
                Text = "Add Student",
                Order = ToolbarItemOrder.Primary,
                Priority = 2
            };
            addStudentToolbarItem.Clicked += OnAddStudentButtonClicked;
            ToolbarItems.Add(addStudentToolbarItem);

            var addDateToolbarItem = new ToolbarItem
            {
                Text = "Add Date",
                Order = ToolbarItemOrder.Primary,
                Priority = 3
            };
            addDateToolbarItem.Clicked += OnAddDateButtonClicked;
            ToolbarItems.Add(addDateToolbarItem);

            // Refresh the grid layout
            GenerateGridLayout();
        }

        private bool ValidateAndSaveChanges()
        {
            bool isValid = true;
            bool hasChanges = false; // Flag to check if there are any changes
            var updates = new List<TimeLogUpdate>();

            foreach (var kvp in entryDictionary)
            {
                var (row, column) = kvp.Key;
                var entry = kvp.Value;

                if (!TimeSpan.TryParse(entry.Text, out TimeSpan time))
                {
                    isValid = false;
                    entry.TextColor = Colors.Red; // Highlight invalid entry
                }
                else
                {
                    var record = AttendanceRecords[row - 1];
                    var date = DateHeaders[column - 2];
                    int totalMinutes = (int)time.TotalMinutes;

                    // Check if the entry value is different from the existing value
                    var existingValue = record.DailyHours[column - 2];
                    string formattedTime = $"{time.Hours:D2}:{time.Minutes:D2}";
                    if (existingValue != formattedTime)
                    {
                        hasChanges = true; // A change has been detected
                    }

                    updates.Add(new TimeLogUpdate
                    {
                        StudentId = record.StudentId,
                        Date = date,
                        totalMinutes = totalMinutes,
                        courseId = record.courseId,
                        WorkDescription = record.WorkDescription
                    });
                }
            }

            if (!hasChanges)
            {
                // No changes detected
                DisplayAlert("Error", "No changes have been made.", "OK");
                return false;
            }

            if (isValid)
            {
                SaveUpdatesToDatabase(updates);
            }

            return isValid;
        }



        private void SaveUpdatesToDatabase(List<TimeLogUpdate> updates)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                try
                {
                    connection.Open();
                    foreach (var update in updates)
                    {
                        // Update the query to match the new database schema
                        string query = @"
                            INSERT INTO time_logs(student_id, log_date, minutes_logged, course_id, work_desc)
                            VALUES (@studentId, @logDate, @totalMinutes, @courseId, @workDescription)
                            ON DUPLICATE KEY UPDATE 
                                minutes_logged = @totalMinutes, 
                                work_desc = @workDescription";

                        using (var command = new MySqlCommand(query, connection))
                        {
                            // Map the parameters to the new table columns
                            command.Parameters.AddWithValue("@studentId", update.StudentId);
                            command.Parameters.AddWithValue("@logDate", update.Date);
                            command.Parameters.AddWithValue("@totalMinutes", update.totalMinutes);
                            command.Parameters.AddWithValue("@courseId", update.courseId);
                            command.Parameters.AddWithValue("@workDescription", update.WorkDescription);

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", $"Database save failed: {ex.Message}", "OK");
                }
            }
        }






        //------------------------------------------------------------Deletion part---------------------------------------------------------------------------------------
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int studentId)
            {
                bool confirmDelete = await DisplayAlert(
                    "Confirm Delete",
                    "Are you sure you want to delete this student and all their logs?",
                    "Yes",
                    "No"
                );

                if (confirmDelete)
                {
                    // Remove student from the in-memory AttendanceRecords list
                    var recordToRemove = AttendanceRecords.FirstOrDefault(record => record.StudentId == studentId);
                    if (recordToRemove != null)
                    {
                        AttendanceRecords.Remove(recordToRemove);
                    }

                    // Try deleting the student's data from the database
                    bool deleteSuccess = false;
                    try
                    {
                        DeleteStudentFromDatabase(studentId);
                        deleteSuccess = true;
                        await DisplayAlert("Success", "Student and their logs have been deleted.", "OK");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete student: {ex.Message}", "OK");
                    }

                    // Refresh the UI to remove the student row
                    GenerateGridLayout();
                }

                // Exit edit mode
                ExitEditMode();
            }
        }

        private void ExitEditMode()
        {
            isEditing = false;

            // Clear the toolbar and rebuild it
            ToolbarItems.Clear();

            // Add the Edit button
            var editToolbarItem = new ToolbarItem
            {
                Text = "Edit",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };
            editToolbarItem.Clicked += OnEditButtonClicked;
            ToolbarItems.Add(editToolbarItem);

            // Add the Add Student button
            var addStudentToolbarItem = new ToolbarItem
            {
                Text = "Add Student",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };
            addStudentToolbarItem.Clicked += OnAddStudentButtonClicked;
            ToolbarItems.Add(addStudentToolbarItem);

            // Add the Add Date button
            var addDateToolbarItem = new ToolbarItem
            {
                Text = "Add Date",
                Order = ToolbarItemOrder.Primary,
                Priority = 2
            };
            addDateToolbarItem.Clicked += OnAddDateButtonClicked;
            ToolbarItems.Add(addDateToolbarItem);

            // Reload the attendance log to refresh the UI
            AttendanceRecords.Clear();
            LoadAttendanceLog();
        }


        private void DeleteStudentFromDatabase(int studentId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete the student's time logs first
                        string deleteLogsQuery = "DELETE FROM time_logs WHERE student_id = @studentId";
                        using (var deleteLogsCommand = new MySqlCommand(deleteLogsQuery, connection, transaction))
                        {
                            deleteLogsCommand.Parameters.AddWithValue("@studentId", studentId);
                            deleteLogsCommand.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch
                    {
                        // Rollback in case of any errors
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        //------------------------------------------------------------Addition of Student---------------------------------------------------------------------------------------
        private async void OnAddStudentButtonClicked(object sender, EventArgs e)
        {
            // Show a custom entry form for student details
            var addStudentPage = new AddStudentPage();
            await Navigation.PushModalAsync(addStudentPage);

            // Wait for the result
            var result = await addStudentPage.GetStudentDetailsAsync();
            if (result == null) return; // User canceled

            try
            {

                // Destructure the result
                var (studentName, netId, utdId) = result.Value;

                string defaultPassword = netId; // Default password
                string defaultUserType = "Student"; // Default user type

                // Save the student to the database
                AddStudentToDatabase(utdId, netId, studentName, defaultPassword, defaultUserType);

                // Refresh the attendance log
                AttendanceRecords.Clear();
                LoadAttendanceLog();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to add the student: " + ex.Message, "OK");
            }
        }


        private void AddStudentToDatabase(int utdId, string netId, string fullName, string defaultPassword, string userType)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Split the full name into first and last names
                        var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        string firstName = nameParts.Length > 0 ? nameParts[0] : "Unknown";
                        string lastName = nameParts.Length > 1 ? nameParts[1] : "Unknown";

                        // Insert into the users table
                        string insertUserQuery = @"
                    INSERT INTO users (utd_id, net_id, first_name, last_name, password, user_role)
                    VALUES (@utdId, @netId, @firstName, @lastName, @password, @userType)";
                        using (var command = new MySqlCommand(insertUserQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@utdId", utdId);
                            command.Parameters.AddWithValue("@netId", netId);
                            command.Parameters.AddWithValue("@firstName", firstName);
                            command.Parameters.AddWithValue("@lastName", lastName);
                            command.Parameters.AddWithValue("@password", defaultPassword);
                            command.Parameters.AddWithValue("@userType", userType);
                            command.ExecuteNonQuery();
                        }

                        // Get the user_id of the newly inserted user
                        long userId;
                        using (var command = new MySqlCommand("SELECT LAST_INSERT_ID()", connection, transaction))
                        {
                            userId = Convert.ToInt64(command.ExecuteScalar());
                        }

                        // Insert into the students table
                        string insertStudentQuery = @"
                    INSERT INTO students (user_id)
                    VALUES (@userId)";
                        using (var command = new MySqlCommand(insertStudentQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@userId", userId);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Failed to add student: " + ex.Message);
                    }
                }
            }
        }


        //------------------------------------------------------------Addition of dates---------------------------------------------------------------------------------------

        private async void OnAddDateButtonClicked(object sender, EventArgs e)
        {
            // Retrieve students from the database
            var students = GetAllStudents();

            if (students.Count == 0)
            {
                await DisplayAlert("Error", "No students found in the database.", "OK");
                return;
            }

            // Ensure ClassName is a valid course ID
            if (!int.TryParse(ClassName, out int courseId))
            {
                await DisplayAlert("Error", "Invalid course ID. Please check the class setup.", "OK");
                return;
            }

            // Show the AddDatePage with the courseId
            var addDatePage = new AddDatePage(students, courseId);
            await Navigation.PushModalAsync(addDatePage);

            // Wait for the result
            var result = await addDatePage.GetTimeLogUpdateAsync();
            if (result == null) return; // User canceled

            try
            {
                // Use result to retrieve UTD ID
                var timeLogUpdate = result;

                // Fetch the student ID using the UTD ID
                int studentId = GetStudentIdFromUtdId(timeLogUpdate.StudentId);

                // Add the data to the database
                AddDateTimeToDatabase(
                    studentId, // Use the fetched student ID
                    timeLogUpdate.courseId,
                    timeLogUpdate.Date,
                    timeLogUpdate.totalMinutes,
                    timeLogUpdate.WorkDescription
                );

                // Refresh the attendance log
                AttendanceRecords.Clear();
                LoadAttendanceLog();

                await DisplayAlert("Success", "Date and time added successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to add the date and time: " + ex.Message, "OK");
            }
        }




        private List<Student> GetAllStudents()
        {
            var students = new List<Student>();

            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = "SELECT utd_id, CONCAT(first_name, ' ', last_name) AS full_name " +
                               "FROM users WHERE user_role = 'Student'" ;

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            UtdId = reader.GetInt32("utd_id"),
                            FullName = reader.GetString("full_name")
                        });
                    }
                }
            }

            return students;
        }


        private int GetStudentIdFromUtdId(int utdid)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                // Query to join the students table with the users table to retrieve student_id
                string query = @"
                                SELECT s.student_id
                                FROM students AS s
                                INNER JOIN users AS u ON s.user_id = u.user_id
                                WHERE u.utd_id = @utdid";

                using (var command = new MySqlCommand(query, connection))
                {
                    // Passing the UTD ID as a parameter
                    command.Parameters.AddWithValue("@utdid", utdid);

                    // Execute the query and retrieve the result
                    var result = command.ExecuteScalar();

                    // If a result is found, return the student_id
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // Throw an exception if no student_id is found
                        throw new Exception($"Student ID not found for UTD ID: {utdid}");
                    }
                }
            }
        }



        private void AddDateTimeToDatabase(int studentId, int courseId, DateTime date, int totalMinutes, string workDesc)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO time_logs (student_id, course_id, log_date, minutes_logged, work_desc)
                    VALUES (@studentId, @courseId, @logDate, @minutesLogged, @workDesc)
                    ON DUPLICATE KEY UPDATE 
                        minutes_logged = @minutesLogged,
                        work_desc = @workDesc";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@studentId", studentId);
                    command.Parameters.AddWithValue("@courseId", courseId);
                    command.Parameters.AddWithValue("@logDate", date.Date); // Only the date part
                    command.Parameters.AddWithValue("@minutesLogged", totalMinutes); // Total minutes logged
                    command.Parameters.AddWithValue("@workDesc", workDesc); // Provided work description

                    command.ExecuteNonQuery();
                }
            }
        }






    }



    public class DailyLog
    {
        public DateTime Date { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    public class TimeLogUpdate
    {
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public int totalMinutes { get; set; }
        public int courseId { get; set; }
        public String WorkDescription { get; set; }
    }

    public class StudentAttendanceRecord
    {
        public int StudentId { get; set; } // Unique identifier for the student
        public string StudentName { get; set; } // Full name of the student
        public string CumulativeHours { get; set; } // Total hours logged as a formatted string
        public List<string> DailyHours { get; set; } // List of daily logged hours as strings
        public int courseId { get; set; } // Course ID associated with the student
        public string WorkDescription { get; set; } // Description of the work done
    }

    public class Student
    {
        public int UtdId { get; set; }
        public string FullName { get; set; }
    }

}
