/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using TPTMauiApp.ViewModels;
using Microsoft.Maui.Controls;

namespace TPTMauiApp.Views
{
    public partial class DataDisplayPage : ContentPage
    {
        public DataDisplayPage(DataDisplayViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}
