using Microsoft.EntityFrameworkCore;
using UniversitySystem2.Models;

namespace UniversitySystem2
{
    public class UniversityContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        public UniversityContext(DbContextOptions<UniversityContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=university.db");
            }
        }
    }
}
