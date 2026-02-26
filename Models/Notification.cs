namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    // This model represents a notification for a user. It can be used to store messages that need to be shown to the user, such as booking confirmations, reminders, or system alerts.
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Foreign Key + Navigation Property
        //public int UserId { get; set; } 
        public User User { get; set; }
    }

}
