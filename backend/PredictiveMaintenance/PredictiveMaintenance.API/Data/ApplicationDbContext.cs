using Microsoft.EntityFrameworkCore;
using PredictiveMaintenance.API.Models;
using System.Reflection.PortableExecutable;

namespace PredictiveMaintenance.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PredictiveMaintenance.API.Models.Machine> Machines { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SensorReading>()
                .HasOne<PredictiveMaintenance.API.Models.Machine>()
                .WithMany()
                .HasForeignKey(s => s.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Prediction>()
                .HasOne<PredictiveMaintenance.API.Models.Machine>()
                .WithMany()
                .HasForeignKey(p => p.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Alert>()
                .HasOne<PredictiveMaintenance.API.Models.Machine>()
                .WithMany()
                .HasForeignKey(a => a.MachineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
