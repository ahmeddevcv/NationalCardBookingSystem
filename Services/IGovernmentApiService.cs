namespace NationalCardBookingSystemWithoutCleanArch.Services
{
    public interface IGovernmentApiService
    {
        Task<string> LoginAndGetSessionAsync(int userId, string phone, string otp);
        Task<bool> IsSessionValidAsync(int userId);
        Task<string> GetSavedSessionAsync(int userId);

        //
        //Task<string> SendOtpAsync(string phoneNumber);
    }
}
