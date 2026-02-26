namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public class User
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        //public string PasswordHash { get; set; }
        public string? GovernmentSessionToken { get; set; }
        public DateTime? SessionExpireAt { get; set; }

        // Relationships
        public List<FamilyMember> FamilyMembers { get; set; } = new();
        public List<BookingSetting> BookingSettings { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();


    }
}
