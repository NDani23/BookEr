using System.ComponentModel.DataAnnotations;

namespace BookEr.Web.Models
{
    public class RegisterViewModel : VisitorViewModel
    {

        [Required(ErrorMessage = "User name is required")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "Invalid user name format or length")]
        public String UserName { get; set; } = null!;


        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "Invalid password format or length")]
        [DataType(DataType.Password)]
        public String UserPassword { get; set; } = null!;


        [Required(ErrorMessage = "Repeated password is required")]
        [Compare(nameof(UserPassword), ErrorMessage = "Missmatch in passwords")]
        [DataType(DataType.Password)]
        public String UserConfirmPassword { get; set; } = null!;
    }
}
