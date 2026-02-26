using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Helpers;
using NationalCardBookingSystemWithoutCleanArch.Models;
using NationalCardBookingSystemWithoutCleanArch.Repositories;

namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        //  Register //based on business
        // Register (Currently simplified - OTP systems usually don't require explicit registration)
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
                    // TODO (Production Improvement):
        // - Consider removing Register if using pure OTP-based authentication.
        // - Add phone number format validation.
        // - Add rate limiting to prevent abuse.
        // - Add logging for security monitoring.

            var existingUser = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

            if (existingUser != null)
                throw new InvalidOperationException("User already exists");

            var user = new User
            {
                PhoneNumber = dto.PhoneNumber
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _jwtHelper.GenerateToken(user);
            return token;
        }

        //  Login //based on business
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = _jwtHelper.GenerateToken(user);
            return token;
        }



        public async Task<string> SendOtpAsync(string phoneNumber)
        {
            var user = await _userRepository.GetByPhoneAsync(phoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber,
                    //GovernmentSessionToken = string.Empty
                };
                try
                {
                    await _userRepository.AddAsync(user);
                    await _userRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException?.Message);
                    throw;
                }

            }
            var fakeOtp = "1234";

            return fakeOtp;
        }
        

        // Verify OTP and issue JWT
        public async Task<string> VerifyOtpAsync(OtpDto dto)
        {
            // Validate OTP (mocked)
            if (dto.OtpCode != "1234")
                throw new UnauthorizedAccessException("Invalid OTP");
            var user = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = dto.PhoneNumber,
                    GovernmentSessionToken = Guid.NewGuid().ToString(),
                    SessionExpireAt = DateTime.UtcNow.AddMinutes(30)
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }
            else
            {
                user.GovernmentSessionToken = Guid.NewGuid().ToString();
                user.SessionExpireAt = DateTime.UtcNow.AddMinutes(30);

                await _userRepository.SaveChangesAsync();
            }

            var token = _jwtHelper.GenerateToken(user);

            return token;
        }
    }
}
