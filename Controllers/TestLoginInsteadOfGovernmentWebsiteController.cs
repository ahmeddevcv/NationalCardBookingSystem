using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NationalCardBookingSystemWithoutCleanArch.Controllers
{
    //[Route("api/[controller]")]
    [Route("login")]
    [ApiController]
    public class TestLoginInsteadOfGovernmentWebsiteController : ControllerBase
    {

        [HttpPost]
        public IActionResult Login([FromForm] string phone, [FromForm] string otp)
        {
            if (otp == "1234") // مجرد مثال
            {
                Response.Cookies.Append("SESSIONID", Guid.NewGuid().ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });
                return Ok(new { message = "Login successful" });
            }

            return Unauthorized(new { message = "Invalid OTP" });
        }
       

    }
}
