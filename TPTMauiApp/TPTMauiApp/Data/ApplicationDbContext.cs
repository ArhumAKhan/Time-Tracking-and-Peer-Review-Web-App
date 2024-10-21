/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */


using Microsoft.EntityFrameworkCore;
using TPTMauiApp.Models;

namespace TPTMauiApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Professor> Professors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Professor -> Course Relationship
            modelBuilder.Entity<Professor>()
                .HasMany(p => p.Courses)
                .WithOne(c => c.Professor)
                .HasForeignKey(c => c.ProfessorId);

            // Course -> TimeLog Relationship
            modelBuilder.Entity<Course>()
                .HasMany(c => c.TimeLogs)
                .WithOne(t => t.Course)
                .HasForeignKey(t => t.CourseId);

            // Student -> TimeLog Relationship
            modelBuilder.Entity<Student>()
                .HasMany(s => s.TimeLogs)
                .WithOne(t => t.Student)
                .HasForeignKey(t => t.StudentId);

            // Set primary keys and optional configuration
            modelBuilder.Entity<TimeLog>()
                .HasKey(t => t.LogId);  // Primary key for TimeLog

            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);  // Primary key for Course

            modelBuilder.Entity<Student>()
                .HasKey(s => s.StudentId);  // Primary key for Student

            modelBuilder.Entity<Professor>()
                .HasKey(p => p.ProfessorId);  // Primary key for Professor

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);  // Primary key for User

            // Ensure unique NetID in User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.NetID)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
