namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public class User
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }

        // OTP
        public string? OtpHash { get; set; }
        public DateTime? OtpExpireAt { get; set; }
        public int FailedOtpAttempts { get; set; }
        public DateTime? LockUntil { get; set; }

        // Session
        public string? GovernmentSessionToken { get; set; }
        public DateTime? SessionExpireAt { get; set; }

        public List<FamilyMember> FamilyMembers { get; set; } = new();
        public List<BookingSetting> BookingSettings { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}
