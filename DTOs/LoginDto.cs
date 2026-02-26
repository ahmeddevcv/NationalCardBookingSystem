using System.ComponentModel.DataAnnotations;

namespace NationalCardBookingSystemWithoutCleanArch.DTOs
{
    public class LoginDto
    {
        //+9647XXXXXXXX
        //✔ +9647711234567
       //❌ +964770123456


        //07XXXXXXXXX
        //✔ 07701234567
        [Required]
        [RegularExpression(@"^(?:\+9647\d{9}|07\d{9})$",
        ErrorMessage = "Invalid Iraqi phone number")]
        // This regex allows for both formats: +9647XXXXXXXXX and 07XXXXXXXXX, ensuring the correct number of digits and valid prefixes.
        public string PhoneNumber { get; set; }
    }
}
