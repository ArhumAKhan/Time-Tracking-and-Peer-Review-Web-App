/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */

using System.Collections.ObjectModel;
using TPTMauiApp.Models;
using TPTMauiApp.Data;
using Microsoft.EntityFrameworkCore;

namespace TPTMauiApp.ViewModels
{
    public class DataDisplayViewModel
    {
        public ObservableCollection<Professor> Professors { get; set; }
        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<TimeLog> TimeLogs { get; set; }

        private readonly ApplicationDbContext _context;

        public DataDisplayViewModel(ApplicationDbContext context)
        {
            _context = context;
            LoadData();
        }

        private void LoadData()
        {
            Professors = new ObservableCollection<Professor>(
                _context.Professors
                .Include(p => p.Courses)
                .ToList()
            );

            Courses = new ObservableCollection<Course>(
                _context.Courses
                    .Include(c => c.Professor)
                    .Include(c => c.TimeLogs)
                    .ToList()
            );

            Students = new ObservableCollection<Student>(
                _context.Students
                    .Include(s => s.TimeLogs)
                    .ToList()
            );

            TimeLogs = new ObservableCollection<TimeLog>(
                _context.TimeLogs
                    .Include(t => t.Student)
                    .Include(t => t.Course)
                    .ToList()
            );
        }
    }
}
