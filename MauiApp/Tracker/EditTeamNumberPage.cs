using MySql.Data.MySqlClient;


namespace Tracker
{
    public class EditTeamNumberPage : ContentPage
    {
        private string ClassName { get; set; }
        private List<StudentTeamRecord> StudentTeams { get; set; }

        public EditTeamNumberPage(string className)
        {
            ClassName = className;
            Title = "Edit Team Numbers";
            StudentTeams = new List<StudentTeamRecord>();

            // Load data before creating the UI
            LoadStudentTeams();

            var saveButton = new Button
            {
                Text = "Save Changes",
                BackgroundColor = Colors.Green,
                TextColor = Colors.White
            };
            saveButton.Clicked += OnSaveChangesClicked;

            // Generate the editor UI after loading data
            var teamEditor = GenerateTeamEditor();

            Content = new StackLayout
            {
                Children = { teamEditor, saveButton }
            };
        }


        private void LoadStudentTeams()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                string query = @"
                SELECT tm.student_id, tm.team_number, 
                       CONCAT(u.first_name, ' ', u.last_name) AS student_name
                FROM team_members tm
                JOIN students s ON tm.student_id = s.student_id
                JOIN users u ON s.user_id = u.user_id
                WHERE tm.course_id = @courseId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@courseId", int.Parse(ClassName));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Loading student: {reader["student_name"]}");
                            StudentTeams.Add(new StudentTeamRecord
                            {
                                StudentId = reader.GetInt32("student_id"),
                                TeamNumber = reader.GetInt32("team_number"),
                                StudentName = reader.GetString("student_name")
                            });
                        }
                    }
                }
            }
        }

        private StackLayout GenerateTeamEditor()
        {
            var teamEditorLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10)
            };

            foreach (var student in StudentTeams)
            {
                var studentLabel = new Label
                {
                    Text = student.StudentName,
                    VerticalOptions = LayoutOptions.Center
                };

                var teamNumberEntry = new Entry
                {
                    Text = student.TeamNumber.ToString(),
                    Keyboard = Keyboard.Numeric,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                // Save changes to the StudentTeams list on text change
                teamNumberEntry.TextChanged += (sender, args) =>
                {
                    if (int.TryParse(args.NewTextValue, out int newTeamNumber))
                    {
                        student.TeamNumber = newTeamNumber;
                    }
                };

                var studentRow = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { studentLabel, teamNumberEntry }
                };

                teamEditorLayout.Children.Add(studentRow);
            }

            return teamEditorLayout;
        }


        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            try
            {
                SaveTeamNumbersToDatabase();
                await DisplayAlert("Success", "Team numbers updated successfully.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update team numbers: {ex.Message}", "OK");
            }
        }

        private void SaveTeamNumbersToDatabase()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            {
                connection.Open();

                foreach (var student in StudentTeams)
                {
                    
                    string query = @"
                        INSERT INTO team_members (student_id, course_id, team_number)
                        VALUES (@studentId, @courseId, @teamNumber)
                        ON DUPLICATE KEY UPDATE team_number = @teamNumber";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", student.StudentId);
                        command.Parameters.AddWithValue("@courseId", int.Parse(ClassName));
                        command.Parameters.AddWithValue("@teamNumber", student.TeamNumber);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating student {student.StudentId}: {ex.Message}");
                        }
                    }
                }
            }
        }

    }

    public class StudentTeamRecord
    {
        public int StudentId { get; set; }
        public int TeamNumber { get; set; }
        public string StudentName { get; set; }
    }

}
