using NationalCardBookingSystemWithoutCleanArch.DTOs;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);


        Task<string> SendOtpAsync(string phoneNumber);
        Task<string> VerifyOtpAsync(OtpDto dto);
    }
}
