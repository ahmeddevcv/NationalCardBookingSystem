using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Services;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.SendOtpAsync(dto.PhoneNumber);

            return Ok(new { Message = "OTP sent successfully" });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpDto dto)
        {
            try
            {
                var token = await _authService.VerifyOtpAsync(dto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
        }


    }
}
