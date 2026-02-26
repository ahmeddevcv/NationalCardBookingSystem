using System.ComponentModel.DataAnnotations;

namespace NationalCardBookingSystemWithoutCleanArch.DTOs
{
    public class FamilyMemberDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [Required]
        [StringLength(20)]
        public string NationalId { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string TransactionType { get; set; }
    }
}
