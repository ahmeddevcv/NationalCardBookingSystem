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

        //  Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _authService.RegisterAsync(dto);
            return Ok(new { Token = token });
        }

        //  Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            return Ok(new { Token = token });
        }


        // Step 1: Enter phone number and send OTP
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            var otp = await _authService.SendOtpAsync(dto.PhoneNumber);

            return Ok(new
            {
                Message = "OTP Sent Successfully",
                Otp = otp 
            });
        }// Note: In production, do not return the OTP in the response. It should be sent via SMS or email.

        // Step 2: Verify OTP and return JWT
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpDto dto)
        {
            try { 
                var token = await _authService.VerifyOtpAsync(dto); 
                return Ok(new { Token = token }); } 
            catch (Exception ex) 
            { return Unauthorized(new { Error = ex.Message }); }
        }
    }
}
