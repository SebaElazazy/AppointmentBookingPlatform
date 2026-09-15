using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppointmentsManagementSystem.Models;

namespace AppointmentsManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffService> StaffServices { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; }
        public DbSet<AvailabilityException> AvailabilityExceptions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StaffService>()
                .HasKey(ss => new { ss.StaffId, ss.ServiceId });

            builder.Entity<StaffService>()
                .HasOne(ss => ss.Staff)
                .WithMany(s => s.StaffServices)
                .HasForeignKey(ss => ss.StaffId);

            builder.Entity<StaffService>()
                .HasOne(ss => ss.Service)
                .WithMany(s => s.StaffServices)
                .HasForeignKey(ss => ss.ServiceId);

            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}