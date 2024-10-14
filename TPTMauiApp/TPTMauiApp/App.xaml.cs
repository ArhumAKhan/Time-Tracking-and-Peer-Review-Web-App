/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using TPTMauiApp.Data;
using TPTMauiApp.Views;

namespace TPTMauiApp;

public partial class App : Application
{
    private readonly ApplicationDbContext _dbContext;

    public App(ApplicationDbContext dbContext)
    {
        InitializeComponent();
        _dbContext = dbContext;

        // Set HoursLoggedView as the initial page for testing
        MainPage = new NavigationPage(new HoursLoggedView(_dbContext));
    }
}
