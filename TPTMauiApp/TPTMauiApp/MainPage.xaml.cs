/*
  Author: Nikhil Giridharan
  NetID: nxg220038
  Date: 9.22.2024
  Class: CS 4485.0W1
  Assignment: Time Tracking Desktop App (Part 2)
 
  C# file defines the code for the MainPage of the time tracking app.
  Initializes the XAML components and does data binding between the view and the ViewModel.
  MainPage uses the MainPageViewModel to handle displaying the list of students, time logs,
  modifying time entries and emailing students.
*/

using System.Text.RegularExpressions;
using TPTMauiApp.Data;

namespace TPTMauiApp;

// MainPage class inherits from ContentPage and is main UI screen for time tracking reporting.
public partial class MainPage : ContentPage
{
    private readonly ApplicationDbContext _dbContext;

    // Constructor for the MainPage class
    public MainPage(ApplicationDbContext dbContext)
    {
        // Sets up the UI components that were declared in the XAML file
        InitializeComponent();
        _dbContext = dbContext;

        // The ViewModel is for handles business logic and provides data to the UI.
        BindingContext = new MainPageViewModel();
    }
}
