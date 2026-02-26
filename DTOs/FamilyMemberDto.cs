using System.ComponentModel.DataAnnotations;

namespace NationalCardBookingSystemWithoutCleanArch.DTOs
{
    public class FamilyMemberDto
    {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string NationalId { get; set; }
    public DateTime BirthDate { get; set; }
    public string TransactionType { get; set; }
    public int UserId { get; set; }
    }
}
