using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Helpers;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<BookingSetting> BookingSettings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  Encryption Converter
            var converter = new EncryptedConverter();

            modelBuilder.Entity<FamilyMember>()
                .Property(x => x.NationalId)
                .HasConversion(converter)
                .HasMaxLength(500);


            modelBuilder.Entity<User>()
                .Property(x => x.GovernmentSessionToken)
                .HasConversion(converter);


            // =========================
            // Relationships
            // =========================

            // User ↔ BookingSetting (One-to-Many)
            modelBuilder.Entity<BookingSetting>()
                .HasOne(bs => bs.User)
                .WithMany(u => u.BookingSettings)
                .HasForeignKey(bs => bs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<BookingSetting>()
            //    .HasIndex(bs => new { bs.UserId, bs.Governorate, bs.Office })
            //    .IsUnique();

            // User ↔ Notification (One-to-Many)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // =========================
            // Indexes (Performance)
            // =========================
            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.UserId);

            modelBuilder.Entity<FamilyMember>()
                .HasIndex(fm => fm.UserId);

            modelBuilder.Entity<BookingSetting>()
                .HasIndex(bs => new { bs.UserId, bs.Governorate, bs.Office })
                .IsUnique();


            // User ↔ FamilyMember (One-to-Many)
            modelBuilder.Entity<FamilyMember>()
                .HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMembers)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            //apointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //index for appointments to optimize queries by user and date
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.UserId, a.AppointmentDate });
        }
    }
}
