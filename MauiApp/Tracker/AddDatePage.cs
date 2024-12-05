using Tracker;

       // ******************************************************************************
        // * Add date page for Tracker Application
        // *
        // * Written by Jaden Nguyen and Arhum Khan for CS 4485.
        // * NetID: jan200003, axk210013
        // *
        // * This page puts all the entries for adding a date for a specific student onto one display
        // * Errors display if incorrect (invalid) information is put in or nothing is put in and tried to be submitted.
        // * Returns the results back to TimeLog.xaml.cs
        // ******************************************************************************
public class AddDatePage : ContentPage
{
    private Picker studentPicker;
    private Entry dateEntry;
    private Entry timeEntry;
    private Entry workDescriptionEntry;
    private Button submitButton;
    private Button cancelButton;

    private TaskCompletionSource<TimeLogUpdate?> tcs;

    private List<Student> AddDateStudents;
    private int AddDateStudentscourseId;

    public AddDatePage(List<Student> students, int courseId)
    {
        AddDateStudents = students;
        AddDateStudentscourseId = courseId; 
        Title = "Add Date";

        // Student Picker
        studentPicker = new Picker { Title = "Select Student" };
        studentPicker.ItemsSource = AddDateStudents.Select(s => s.FullName).ToList();

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
    
        // ******************************************************************************
        // * Handles the submission of the page
        // * Validates the input and creates the time log to be updated
        // ******************************************************************************
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

        var selectedStudent = AddDateStudents[studentPicker.SelectedIndex];

        // Create TimeLogUpdate object to return
        var timeLogUpdate = new TimeLogUpdate
        {
            StudentId = selectedStudent.UtdId,
            Date = selectedDate,
            totalMinutes = (int)selectedTime.TotalMinutes,
            WorkDescription = workDescriptionEntry.Text,
            courseId = AddDateStudentscourseId 
        };

        tcs?.TrySetResult(timeLogUpdate);
        Navigation.PopModalAsync();
    }

    // ******************************************************************************
    // * Handles the cancelation of the page
    // * Returns null and from TimeLog.xaml.cs, it returns to the previous page.
    // ******************************************************************************
    private void OnCancelClicked(object sender, EventArgs e)
    {
        tcs?.TrySetResult(null);
        Navigation.PopModalAsync();
    }


    // ******************************************************************************
    // * Returns a task to await the result of the form submission
    // * The task contains a tuple with the TimeLogUpdate object to be updated and if it is canceled then it returns null.
    // ******************************************************************************
    public Task<TimeLogUpdate?> GetTimeLogUpdateAsync()
    {
        tcs = new TaskCompletionSource<TimeLogUpdate?>();
        return tcs.Task;
    }
}
