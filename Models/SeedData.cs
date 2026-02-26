using NationalCardBookingSystemWithoutCleanArch.Data;

namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var random = new Random();

            // =========================
            // Users
            // =========================
            var users = new List<User>();

            for (int i = 1; i <= 5; i++)
            {
                users.Add(new User
                {
                    PhoneNumber = $"0100000000{i}",
                    FailedOtpAttempts = 0
                });
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            // =========================
            // Family Members
            // =========================
            var familyMembers = new List<FamilyMember>();

            foreach (var user in users)
            {
                for (int i = 1; i <= random.Next(1, 4); i++)
                {
                    familyMembers.Add(new FamilyMember
                    {
                        UserId = user.Id,
                        FullName = $"Family Member {i} - User {user.Id}",
                        NationalId = $"{random.Next(100000000, 999999999)}{random.Next(1000, 9999)}",
                        BirthDate = DateTime.Now.AddYears(-random.Next(5, 50)),
                        TransactionType = random.Next(0, 2) == 0 ? "Passport" : "National ID"
                    });
                }
            }

            context.FamilyMembers.AddRange(familyMembers);
            context.SaveChanges();

            // =========================
            // Booking Settings
            // =========================
            var governorates = new[] { "Cairo", "Giza", "Alexandria" };
            var offices = new[] { "Main Office", "Branch A", "Branch B" };

            var bookingSettings = new List<BookingSetting>();

            foreach (var user in users)
            {
                var gov = governorates[random.Next(governorates.Length)];
                var office = offices[random.Next(offices.Length)];

                bookingSettings.Add(new BookingSetting
                {
                    UserId = user.Id,
                    Governorate = gov,
                    Office = office,
                    FamilyCount = user.FamilyMembers.Count,
                    AutoBookingEnabled = random.Next(0, 2) == 1
                });
            }

            context.BookingSettings.AddRange(bookingSettings);
            context.SaveChanges();

            // =========================
            // Appointments
            // =========================
            var appointments = new List<Appointment>();

            foreach (var user in users)
            {
                for (int i = 0; i < 3; i++)
                {
                    var isBooked = random.Next(0, 2) == 1;

                    appointments.Add(new Appointment
                    {
                        UserId = user.Id,
                        Governorate = governorates[random.Next(governorates.Length)],
                        Office = offices[random.Next(offices.Length)],
                        AppointmentDate = DateTime.Today.AddDays(random.Next(1, 30)),
                        IsBooked = isBooked,
                        BookedAt = isBooked ? DateTime.UtcNow.AddDays(-random.Next(1, 5)) : null
                    });
                }
            }

            context.Appointments.AddRange(appointments);
            context.SaveChanges();

            // =========================
            // Notifications
            // =========================
            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Message = "Your booking request has been received.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Message = "New appointments are now available.",
                    IsRead = random.Next(0, 2) == 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });
            }

            context.Notifications.AddRange(notifications);
            context.SaveChanges();
        }
    }
}
