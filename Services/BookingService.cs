using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.Hubs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly IGovernmentApiService _govService;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext context,
                              IGovernmentApiService govService,
                              IHubContext<NotificationHub> hub, 
                              ILogger<BookingService> logger)
        {
            _context = context;
            _govService = govService;
            _hub = hub;
            _logger = logger;
        }

        // Called by Hangfire job
        public async Task CheckAvailableAppointments()
        {
            _logger.LogInformation("Checking available appointments...");

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                try
                {
                    var sessionValid = await _govService.IsSessionValidAsync(user.Id);

                    if (!sessionValid)
                    {
                        _logger.LogWarning($"Session expired for user {user.Id}");
                        continue;
                    }

                    var available = await HasAvailableAppointmentsAsync(user.Id);

                    if (available)
                    {
                        _logger.LogInformation($"Appointment found for user {user.Id}");

                        // Save notification in DB
                        var notification = new Notification
                        {
                            UserId = user.Id,
                            Message = "🔥 تم فتح موعد جديد!"
                        };

                        await _context.Notifications.AddAsync(notification);
                        await _context.SaveChangesAsync();

                        // Send real-time notification via SignalR
                        await _hub.Clients.User(user.Id.ToString())
                            .SendAsync("ReceiveNotification", notification.Message);

                        var setting = await _context.BookingSettings
                            .FirstOrDefaultAsync(x => x.UserId == user.Id);

                        if (setting != null && setting.AutoBookingEnabled)
                        {
                            await ExecuteBookingAsync(user.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while checking appointments for user {user.Id}");
                }
            }
        }


        // Simulate appointment check (will be replaced later with real request)
        public async Task<bool> HasAvailableAppointmentsAsync(int userId)
        {
            await Task.Delay(500); // Simulate latency

            // Here you will send a real request to the government site 
            // Temporary: returns true every 30 seconds (for testing)
            return DateTime.UtcNow.Second % 30 == 0;
        }

        // تنفيذ الحجز الفوري
        public async Task ExecuteBookingAsync(int userId)
        {
            var session = await _govService.GetSavedSessionAsync(userId);

            if (string.IsNullOrEmpty(session))
                throw new Exception("Session not found");

            // محاكاة الحجز
            await Task.Delay(1000);

            // حفظ إشعار نجاح الحجز
            var notification = new Notification
            {
                UserId = userId,
                Message = "✅ تم تنفيذ الحجز بنجاح!"
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // Real-time notification
            await _hub.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notification.Message);
        }
    }


}
