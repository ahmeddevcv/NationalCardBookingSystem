using Microsoft.EntityFrameworkCore;
using NationalCardBookingSystemWithoutCleanArch.Data;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class BookingSettingService : IBookingSettingService
    {
        private readonly AppDbContext _context;

        public BookingSettingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveSettingsAsync(int userId, BookingSettingDto dto)
        {
            var setting = await _context.BookingSettings
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (setting == null)
            {
                setting = new BookingSetting
                {
                    UserId = userId,
                    Governorate = dto.Governorate,
                    Office = dto.Office,
                    FamilyCount = dto.FamilyCount,
                    AutoBookingEnabled = dto.AutoBookingEnabled
                };

                await _context.BookingSettings.AddAsync(setting);
            }
            else
            {
                setting.Governorate = dto.Governorate;
                setting.Office = dto.Office;
                setting.FamilyCount = dto.FamilyCount;
                setting.AutoBookingEnabled = dto.AutoBookingEnabled;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<BookingSetting> GetSettingsAsync(int userId)
        {
            return await GetByUserIdAsync(userId);
        }

        public async Task ToggleAutoBookingAsync(int userId, bool enabled)
        {
            var setting = await GetByUserIdAsync(userId);

            if (setting == null)
                throw new InvalidOperationException("Booking settings not found");

            setting.AutoBookingEnabled = enabled;
            await _context.SaveChangesAsync();
        }
        private Task<BookingSetting?> GetByUserIdAsync(int userId)
        {
            return _context.BookingSettings
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
