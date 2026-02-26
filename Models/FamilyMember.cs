namespace NationalCardBookingSystemWithoutCleanArch.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string NationalId { get; set; }
        public DateTime BirthDate { get; set; }
        public string TransactionType { get; set; }

        // Foreign Key + Navigation Property
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
