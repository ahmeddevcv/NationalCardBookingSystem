namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IBookingService
    {
        Task CheckAvailableAppointments();
        Task<bool> HasAvailableAppointmentsAsync(int userId);
        Task ExecuteBookingAsync(int userId);
    }
}
