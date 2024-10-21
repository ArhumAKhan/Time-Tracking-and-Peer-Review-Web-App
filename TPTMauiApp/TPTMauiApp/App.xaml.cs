/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

namespace TPTMauiApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
