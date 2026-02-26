namespace NationalCardBookingSystemWithoutCleanArch.DTOs
{
    public class BookingSettingDto
    {
        public string Governorate { get; set; }
        public string Office { get; set; }
        public int FamilyCount { get; set; }
        public bool AutoBookingEnabled { get; set; }
    }
}
