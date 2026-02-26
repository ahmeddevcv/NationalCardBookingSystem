using NationalCardBookingSystemWithoutCleanArch.Data;

namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public static class SeedLargeData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            var random = new Random();

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var users = new List<User>();

            for (int i = 1; i <= 50; i++)
            {
                var useInternational = random.Next(0, 2) == 1;

                string phone = useInternational
                    ? $"+9647{random.Next(100000000, 999999999)}"
                    : $"07{random.Next(100000000, 999999999)}";

                users.Add(new User
                {
                    PhoneNumber = phone,
                    FailedOtpAttempts = 0
                });
            }


            context.Users.AddRange(users);
            context.SaveChanges();

            var governorates = new[] { "Cairo", "Giza", "Alexandria", "Dakahlia", "Sharqia" };
            var offices = new[] { "Main Office", "Branch A", "Branch B", "Central Office" };

            var familyMembers = new List<FamilyMember>();
            var bookingSettings = new List<BookingSetting>();
            var appointments = new List<Appointment>();
            var notifications = new List<Notification>();

            foreach (var user in users)
            {
                // ================= Family Members (1-5)
                int familyCount = random.Next(1, 6);

                for (int i = 0; i < familyCount; i++)
                {
                    familyMembers.Add(new FamilyMember
                    {
                        UserId = user.Id,
                        FullName = $"User{user.Id} Member{i}",
                        NationalId = $"{random.Next(100000000, 999999999)}{random.Next(1000, 9999)}",
                        BirthDate = DateTime.Now.AddYears(-random.Next(5, 60)),
                        TransactionType = random.Next(0, 2) == 0 ? "Passport" : "National ID"
                    });
                }

                // ================= Booking Setting (1 per user)
                var gov = governorates[random.Next(governorates.Length)];
                var office = offices[random.Next(offices.Length)];

                bookingSettings.Add(new BookingSetting
                {
                    UserId = user.Id,
                    Governorate = gov,
                    Office = office,
                    FamilyCount = familyCount,
                    AutoBookingEnabled = random.Next(0, 2) == 1
                });

                // ================= Appointments (3-7 per user)
                int appointmentCount = random.Next(3, 8);

                for (int i = 0; i < appointmentCount; i++)
                {
                    var isBooked = random.Next(0, 2) == 1;

                    appointments.Add(new Appointment
                    {
                        UserId = user.Id,
                        Governorate = governorates[random.Next(governorates.Length)],
                        Office = offices[random.Next(offices.Length)],
                        AppointmentDate = DateTime.Today.AddDays(random.Next(1, 60)),
                        IsBooked = isBooked,
                        BookedAt = isBooked ? DateTime.UtcNow.AddDays(-random.Next(1, 10)) : null
                    });
                }

                // ================= Notifications (2 per user)
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Message = "Booking created successfully.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });

                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Message = "New appointment available.",
                    IsRead = random.Next(0, 2) == 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });
            }

            context.FamilyMembers.AddRange(familyMembers);
            context.BookingSettings.AddRange(bookingSettings);
            context.Appointments.AddRange(appointments);
            context.Notifications.AddRange(notifications);

            context.SaveChanges();

            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}
