using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace TimeTrackApp
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ProfessorClasses { get; set; }
        public string SelectedClass { get; set; }

        public ObservableCollection<StudentHours> StudentsHours { get; set; }

        public int TotalWeekHours { get; set; }

        public ICommand ModifyTimeCommand { get; }
        public ICommand EmailStudentsCommand { get; }

        public MainPageViewModel()
        {
            // Dummy data for demonstration
            ProfessorClasses = new ObservableCollection<string>
            {
                "CS4485 - Fall 2024",
                "CS3345 - Spring 2024"
            };

            StudentsHours = new ObservableCollection<StudentHours>
            {
                new StudentHours { StudentName = "John Doe", Day1Hours = "02:00", Day2Hours = "01:30", TotalHours = "03:30" },
                new StudentHours { StudentName = "Jane Smith", Day1Hours = "04:00", Day2Hours = "02:00", TotalHours = "06:00" }
            };

            // Total hours for the week (for demonstration purposes)
            TotalWeekHours = 9;

            ModifyTimeCommand = new Command(ModifyTime);
            EmailStudentsCommand = new Command(EmailStudents);
        }

        // Logic for modifying time (to be implemented)
        void ModifyTime()
        {
            // Implement logic for modifying time
        }

        // Logic for emailing students (to be implemented)
        void EmailStudents()
        {
            // Implement logic for sending emails to students
        }

        // INotifyPropertyChanged implementation for data binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // StudentHours class to store each student's logged time data
    public class StudentHours
    {
        public string StudentName { get; set; }
        public string Day1Hours { get; set; }
        public string Day2Hours { get; set; }
        public string TotalHours { get; set; }
    }
}
