namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public string Governorate { get; set; }
        public string Office { get; set; }
        public DateTime AppointmentDate { get; set; }

        public bool IsBooked { get; set; } = false;
        public DateTime? BookedAt { get; set; }

        // FK
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
