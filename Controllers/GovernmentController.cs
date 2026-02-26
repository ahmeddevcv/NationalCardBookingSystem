using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalCardBookingSystemWithoutCleanArch.Services;
using System.Security.Claims;
using NationalCardBookingSystemWithoutCleanArch.DTOs;


namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GovernmentController : ControllerBase
    {
        private readonly IGovernmentApiService _service;

        public GovernmentController(IGovernmentApiService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        // Login to the govenment website
        [HttpPost("login")]
        public async Task<IActionResult> Login(GovernmentLoginDto dto)
        {
            try
            {
                var userId = GetUserId();
                var session = await _service.LoginAndGetSessionAsync(userId, dto.PhoneNumber, dto.OtpCode);

                return Ok(new
                {
                    Message = "Government session created",
                    Session = session
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }// This endpoint will be called by the background service to refresh the session
        }

        // check status of session
        [HttpGet("session-status")]
        public async Task<IActionResult> CheckSession()
        {
            var userId = GetUserId();
            var isValid = await _service.IsSessionValidAsync(userId);

            return Ok(new { IsValid = isValid });
        }
    }
}
