using Microsoft.EntityFrameworkCore;
using TaskAPI.Models;

namespace TaskAPI.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Specialization> Specializations { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    }
}
