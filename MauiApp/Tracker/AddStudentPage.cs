public class AddStudentPage : ContentPage
{
    private Entry nameEntry;
    private Entry netIdEntry;
    private Entry utdIdEntry;
    private TaskCompletionSource<(string, string, int)?> tcs;

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

    private void OnSubmitClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameEntry.Text) ||
            string.IsNullOrWhiteSpace(netIdEntry.Text) ||
            !int.TryParse(utdIdEntry.Text, out int utdId) || utdId <= 0)
        {
            DisplayAlert("Error", "Please fill out all fields correctly.", "OK");
            return;
        }

        tcs?.TrySetResult((nameEntry.Text, netIdEntry.Text, utdId));
        Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        tcs?.TrySetResult(null);
        Navigation.PopModalAsync();
    }

    public Task<(string, string, int)?> GetStudentDetailsAsync()
    {
        tcs = new TaskCompletionSource<(string, string, int)?>();
        return tcs.Task;
    }
}
