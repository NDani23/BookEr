using System.ComponentModel.DataAnnotations;

namespace BookEr.DTO
{
    public class RegisterDto
    {

        [Required(ErrorMessage = "Giving a name is required")]
        public string Name { get; set; } = null!;


        [Required(ErrorMessage = "Giving a user name is required")]
        public string UserName { get; set; } = null!;


        [Required(ErrorMessage = "Giving an email is required")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Giving a phone number is required")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Giving a password is required")]
        public string Password { get; set; } = null!;
    }
}
