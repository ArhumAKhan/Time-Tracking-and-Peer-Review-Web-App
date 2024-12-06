
// ******************************************************************************
// * Add date page for Tracker Application
// *
// * Written completely in collaberation by Jaden Nguyen and Arhum Khan for CS 4485.
// * NetID: jan200003, axk210013
// *
// * This page puts all the entries for adding a student with the users entries
// * Errors display if incorrect (invalid) information is put in or nothing is put in and tried to be submitted.
// * Returns the results back to TimeLog.xaml.cs
// ******************************************************************************
public class AddStudentPage : ContentPage
{
    private Entry nameEntry;
    private Entry netIdEntry;
    private Entry utdIdEntry;
    private Entry teamNumberEntry; // Add team number field
    private TaskCompletionSource<(string, string, int, int?)?> tcsStudent;

    public AddStudentPage()
    {
        Title = "Add Student";

        // Create input fields
        nameEntry = new Entry { Placeholder = "Student Name" };
        netIdEntry = new Entry { Placeholder = "NetID" };
        utdIdEntry = new Entry { Placeholder = "UTD ID", Keyboard = Keyboard.Numeric };
        teamNumberEntry = new Entry { Placeholder = "Team Number (Enter 0 for no team)", Keyboard = Keyboard.Numeric }; // Team number field

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
                teamNumberEntry,
                submitButton,
                cancelButton
            }
        };
    }

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
            string.IsNullOrWhiteSpace(netIdEntry.Text) ||
            !int.TryParse(utdIdEntry.Text, out int utdId) || utdId <= 0 ||
            !int.TryParse(teamNumberEntry.Text, out int teamNumber))
        {
            DisplayAlert("Error", "Please fill out all fields correctly.", "OK");
            return;
        }

        tcsStudent?.TrySetResult((nameEntry.Text, netIdEntry.Text, utdId, teamNumber));
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        tcsStudent?.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    public Task<(string, string, int, int?)?> GetStudentDetailsAsync()
    {
        tcsStudent = new TaskCompletionSource<(string, string, int, int?)?>();
        return tcsStudent.Task;
    }
}
