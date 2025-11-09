using System.ComponentModel.DataAnnotations;

namespace BookEr.Web.Models
{
    public class VisitorViewModel
    {
        [Required(ErrorMessage ="Giving a name is required.")]
        [MaxLength(60, ErrorMessage = "The name can contain 60 characters at max.")]
        public String VisitorName { get; set; } = null!;

        [Required(ErrorMessage = "Giving an email is required")]
        [EmailAddress(ErrorMessage = "This is not a valid email format.")]
        [DataType(DataType.EmailAddress)]
        public String VisitorEmail { get; set; } = null!;

        [Required(ErrorMessage = "Giving an adress is required.")]
        public String VisitorAddress { get; set; } = null!;

        [Required(ErrorMessage = "Giving a phone number is required")]
        [Phone(ErrorMessage = "This is not a valid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public String VisitorPhoneNumber { get; set; } = null!;

    }
}
