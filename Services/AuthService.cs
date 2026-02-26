using NationalCardBookingSystemWithoutCleanArch.DTOs;
using NationalCardBookingSystemWithoutCleanArch.Helpers;
using NationalCardBookingSystemWithoutCleanArch.Models;
using NationalCardBookingSystemWithoutCleanArch.Repositories;
using System.Security.Cryptography;
using BCrypt.Net;



namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        private const int OtpExpiryMinutes = 5;
        private const int SessionExpiryMinutes = 30;
        private const int MaxFailedAttempts = 5;
        private const int LockMinutes = 10;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }
        #region depending on business
        ////  Register //based on business
        //// Register (Currently simplified - OTP systems usually don't require explicit registration)
        //public async Task<string> RegisterAsync(RegisterDto dto)
        //{

        //    var existingUser = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

        //    if (existingUser != null)
        //        throw new InvalidOperationException("User already exists");

        //    var user = new User
        //    {
        //        PhoneNumber = dto.PhoneNumber
        //    };

        //    await _userRepository.AddAsync(user);
        //    await _userRepository.SaveChangesAsync();

        //    var token = _jwtHelper.GenerateToken(user);
        //    return token;
        //}

        ////  Login //based on business
        //public async Task<string> LoginAsync(LoginDto dto)
        //{
        //    var user = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

        //    if (user == null)
        //        throw new UnauthorizedAccessException("Invalid credentials");

        //    var token = _jwtHelper.GenerateToken(user);
        //    return token;
        //}

        #endregion


        // ==============================
        // Send OTP
        // ==============================
        public async Task<string> SendOtpAsync(string phoneNumber)
        {
            var user = await _userRepository.GetByPhoneAsync(phoneNumber);

            if (user == null)
            {
                user = new User
                {
                    PhoneNumber = phoneNumber
                };

                await _userRepository.AddAsync(user);
            }

            // Generate secure 6-digit OTP
            var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

            // Hash OTP before saving
            user.OtpHash = BCrypt.Net.BCrypt.HashPassword(otp);
            user.OtpExpireAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes);

            // Reset attempts
            user.FailedOtpAttempts = 0;
            user.LockUntil = null;

            await _userRepository.SaveChangesAsync();

            // TODO: Send OTP via SMS provider
            Console.WriteLine($"OTP for {phoneNumber}: {otp}");
            return "OTP Sent";
        }

        // ==============================
        // Verify OTP
        // ==============================
        public async Task<string> VerifyOtpAsync(OtpDto dto)
        {
            var user = await _userRepository.GetByPhoneAsync(dto.PhoneNumber);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid phone number");

            // Check if locked
            if (user.LockUntil.HasValue && user.LockUntil > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account temporarily locked. Try later.");

            // Check expiration
            if (user.OtpExpireAt == null || user.OtpExpireAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("OTP expired");

            // Verify OTP
            if (user.OtpHash == null || !BCrypt.Net.BCrypt.Verify(dto.OtpCode, user.OtpHash))
            {
                user.FailedOtpAttempts++;

                if (user.FailedOtpAttempts >= MaxFailedAttempts)
                {
                    user.LockUntil = DateTime.UtcNow.AddMinutes(LockMinutes);
                    user.FailedOtpAttempts = 0;
                }

                await _userRepository.SaveChangesAsync();
                throw new UnauthorizedAccessException("Invalid OTP");
            }

            // SUCCESS → Clear OTP
            user.OtpHash = null;
            user.OtpExpireAt = null;
            user.FailedOtpAttempts = 0;
            user.LockUntil = null;

            // Create session
            user.GovernmentSessionToken = Guid.NewGuid().ToString();
            user.SessionExpireAt = DateTime.UtcNow.AddMinutes(SessionExpiryMinutes);

            await _userRepository.SaveChangesAsync();

            // Generate JWT
            return _jwtHelper.GenerateToken(user);
        }
    


}
}
