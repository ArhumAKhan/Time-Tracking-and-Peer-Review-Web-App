/*
 * Author: Johnny An
 * NetID: HXA210014
 * Date: 10.06.24
 * Class: CS4485.0W1
 * Assignment: Time Tracking Desktop App (Part 2)
 * */


using Microsoft.EntityFrameworkCore;
using TPTMauiApp.Models;

namespace TPTMauiApp.Data;

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
        modelBuilder.Entity<Professor>()
            .HasMany(p => p.Courses)
            .WithOne(c => c.Professor)
            .HasForeignKey(c => c.ProfessorId);

        modelBuilder.Entity<Course>()
            .HasMany(c => c.TimeLogs)
            .WithOne(t => t.Course)
            .HasForeignKey(t => t.CourseId);

        modelBuilder.Entity<Student>()
            .HasMany(s => s.TimeLogs)
            .WithOne(t => t.Student)
            .HasForeignKey(t => t.StudentId);

        base.OnModelCreating(modelBuilder);
    }
}

