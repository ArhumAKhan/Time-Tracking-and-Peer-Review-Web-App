
   // ******************************************************************************
        // * Add date page for Tracker Application
        // *
        // * Written by Jaden Nguyen for CS 4485.
        // * NetID: jan200003
        // *
        // * This page puts all the entries for adding a student with the users entries
        // * Errors display if incorrect (invalid) information is put in or nothing is put in and tried to be submitted.
        // * Returns the results back to TimeLog.xaml.cs
        // ******************************************************************************
public class AddStudentPage : ContentPage
{
    // Input fields and taskCompletionSource to handle / return the results 
    private Entry nameEntry;
    private Entry netIdEntry;
    private Entry utdIdEntry;
    private TaskCompletionSource<(string, string, int)?> tcsStudent;

    public AddStudentPage()
    {
        Title = "Add Student";

        // Create input fields
        nameEntry = new Entry { Placeholder = "Student Name" };
        netIdEntry = new Entry { Placeholder = "NetID" };
        utdIdEntry = new Entry { Placeholder = "UTD ID", Keyboard = Keyboard.Numeric };

        // Create buttons
        var submitButton = new Button { Text = "Submit" };
        submitButton.Clicked += OnSubmitClicked;

        var cancelButton = new Button { Text = "Cancel" };
        cancelButton.Clicked += OnCancelClicked;

        // Layout
        Content = new StackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children = {
                new Label { Text = "Enter Student Details", FontSize = 20, HorizontalOptions = LayoutOptions.Center },
                nameEntry,
                netIdEntry,
                utdIdEntry,
                submitButton,
                cancelButton
            }
        };
    }

    // ******************************************************************************
    // * Handles the submission of the page
    // * Validates the input and sends the student info to be updated
    // ******************************************************************************
    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
            string.IsNullOrWhiteSpace(netIdEntry.Text) ||
            !int.TryParse(utdIdEntry.Text, out int utdId) || utdId <= 0)
        {
            DisplayAlert("Error", "Please fill out all fields correctly.", "OK");
            return;
        }

        tcsStudent?.TrySetResult((nameEntry.Text, netIdEntry.Text, utdId));
        Navigation.PopModalAsync();
    }
    
    // ******************************************************************************
    // * Handles the cancelation of the page
    // * Returns null and from TimeLog.xaml.cs, it returns to the previous page.
    // ******************************************************************************
    private void OnCancelClicked(object sender, EventArgs e)
    {
        tcsStudent?.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    // ******************************************************************************
    // * Returns a task to await the result of the form submission
    // * The task contains a tuple with the student name, Netid and Utd id, and if it is canceled then it returns null.
    // ******************************************************************************

    public Task<(string, string, int)?> GetStudentDetailsAsync()
    {
        tcsStudent = new TaskCompletionSource<(string, string, int)?>();
        return tcsStudent.Task;
    }
}
