using Tracker;
public class AddDatePage : ContentPage
{
    private Picker studentPicker;
    private Entry dateEntry;
    private Entry timeEntry;
    private Entry workDescriptionEntry;
    private Button submitButton;
    private Button cancelButton;

    private TaskCompletionSource<TimeLogUpdate?> tcs;

    private List<Student> _students;
    private int _courseId;

    public AddDatePage(List<Student> students, int courseId)
    {
        _students = students;
        _courseId = courseId; // Save the course ID
        Title = "Add Date";

        // Student Picker
        studentPicker = new Picker { Title = "Select Student" };
        studentPicker.ItemsSource = _students.Select(s => s.FullName).ToList();

        // Date Entry
        dateEntry = new Entry { Placeholder = "Date (MM/dd/yyyy)", Keyboard = Keyboard.Text };

        // Time Entry
        timeEntry = new Entry { Placeholder = "Time (HH:mm)", Keyboard = Keyboard.Text };

        // Work Description Entry
        workDescriptionEntry = new Entry { Placeholder = "Work Description" };

        // Submit Button
        submitButton = new Button { Text = "Submit" };
        submitButton.Clicked += OnSubmitClicked;

        // Cancel Button
        cancelButton = new Button { Text = "Cancel" };
        cancelButton.Clicked += OnCancelClicked;

        // Layout
        Content = new StackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children = {
                new Label { Text = "Add Date and Time Log", FontSize = 20, HorizontalOptions = LayoutOptions.Center },
                studentPicker,
                dateEntry,
                timeEntry,
                workDescriptionEntry,
                submitButton,
                cancelButton
            }
        };
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (studentPicker.SelectedIndex == -1)
        {
            DisplayAlert("Error", "Please select a student.", "OK");
            return;
        }

        if (!DateTime.TryParse(dateEntry.Text, out DateTime selectedDate))
        {
            DisplayAlert("Error", "Invalid date format. Please use MM/dd/yyyy.", "OK");
            return;
        }

        if (!TimeSpan.TryParse(timeEntry.Text, out TimeSpan selectedTime))
        {
            DisplayAlert("Error", "Invalid time format. Please use HH:mm.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(workDescriptionEntry.Text))
        {
            DisplayAlert("Error", "Work description cannot be empty.", "OK");
            return;
        }

        var selectedStudent = _students[studentPicker.SelectedIndex];

        // Create TimeLogUpdate object to return
        var timeLogUpdate = new TimeLogUpdate
        {
            StudentId = selectedStudent.UtdId,
            Date = selectedDate,
            totalMinutes = (int)selectedTime.TotalMinutes,
            WorkDescription = workDescriptionEntry.Text,
            courseId = _courseId // Use the course ID from the constructor
        };

        tcs?.TrySetResult(timeLogUpdate);
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        tcs?.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    public Task<TimeLogUpdate?> GetTimeLogUpdateAsync()
    {
        tcs = new TaskCompletionSource<TimeLogUpdate?>();
        return tcs.Task;
    }
}
