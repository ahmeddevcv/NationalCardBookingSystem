using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Models;
using NationalCardBookingSystemWithoutCleanArch.Services;
using System.Security.Claims;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingSettingsController : ControllerBase
    {
        private readonly IBookingSettingService _service;

        public BookingSettingsController(IBookingSettingService service)
        {
            _service = service;
        }


        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing User ID claim");

            return userId;
        }

        // 1️ Save or Update Settings
        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] BookingSettingDto dto)
        {
            var userId = GetUserId();
            await _service.SaveSettingsAsync(userId, dto);
            return Ok(new { message = "Settings saved successfully", settings = dto });
        }

        // 2️ Get Settings
        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var userId = GetUserId();
            var settings = await _service.GetSettingsAsync(userId);
            if (settings == null)
                return NotFound("Booking settings not found");
            return Ok(settings);
        }

        // 3️ Toggle Auto Booking
        [HttpPut("toggle-auto-booking")]
        public async Task<IActionResult> ToggleAutoBooking([FromBody] bool enabled)
        {
            var userId = GetUserId();
            try
            {
                await _service.ToggleAutoBookingAsync(userId, enabled);
                return Ok("Auto booking updated");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
