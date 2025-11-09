using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BookEr.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User name is required")]
        [DisplayName("Name")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
