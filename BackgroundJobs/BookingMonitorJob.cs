using NationalCardBookingSystemWithoutCleanArch.Services;

namespace NationalCardBookingSystemWithoutCleanArch.BackgroundJobs
{
    public class BookingMonitorJob
    {
        private readonly IBookingService _bookingService;

        public BookingMonitorJob(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // دي اللي Hangfire بيناديها
        public async Task CheckAppointments()
        {
            await _bookingService.CheckAvailableAppointments();
        }
    }
}
