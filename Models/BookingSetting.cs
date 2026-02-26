namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public class BookingSetting
    {
        public int Id { get; set; }
        public string Governorate { get; set; }
        public string Office { get; set; }
        public int FamilyCount { get; set; }
        public bool AutoBookingEnabled { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; } // Navigation Property
    }
}
