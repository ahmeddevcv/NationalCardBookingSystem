using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IBookingSettingService
    {
        Task SaveSettingsAsync(int userId, BookingSettingDto dto);
        Task<BookingSetting> GetSettingsAsync(int userId);
        Task ToggleAutoBookingAsync(int userId, bool enabled);
    }
}
