using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookEr.DTO;
using BookEr.Persistence;
using Microsoft.AspNetCore.Authorization;
using BookEr.Web.Models;

namespace BookEr.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookerService _service;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IBookerService service)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto userDto)
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();

            var user = await _userManager.FindByNameAsync(userDto.UserName);

            if(user == null)
            {
                return Unauthorized();
            }
            var userRoles = await _userManager.GetRolesAsync(user);

            if(!userRoles.Contains("Librarian")) 
            {
                return Unauthorized();
            }

            var result = await _signInManager.PasswordSignInAsync(userDto.UserName, userDto.Password,
                isPersistent: false, lockoutOnFailure: false);


            if (result.Succeeded)
            {
                return Ok();
            }

            return Unauthorized("Login was unsuccesfull");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto userDto)
        {
            if (_signInManager.IsSignedIn(User))
                await _signInManager.SignOutAsync();

            if(await _userManager.FindByNameAsync(userDto.UserName) != null)
            {
                return NotFound("User name already exists!");
            }

            if (await _userManager.FindByEmailAsync(userDto.Email) != null)
            {
                return NotFound("Email already exists!");
            }

            var passwordValidator = new PasswordValidator<ApplicationUser>();
            if(userDto.Password != null)
            {
                var isPasswordValid = await passwordValidator.ValidateAsync(_userManager, null, userDto.Password);
                if (!isPasswordValid.Succeeded)
                {
                    return BadRequest("Password must contain at least: 8 characters, 1 digit, 1 upper case");
                }
            }



            var AppUser = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Name = userDto.Name,

            };

            Librarian librarian = new Librarian
            {
                ApplicationUser = AppUser,
            };
            
            var result = await _userManager.CreateAsync(AppUser, userDto.Password);


            if (!result.Succeeded)
            {
                return BadRequest();
            }
            await _userManager.AddToRoleAsync(AppUser, "Librarian");
            var newVisitor = _service.AddLibrarian(librarian);
            await _signInManager.SignInAsync(AppUser, false);


            if (result.Succeeded)
            {
                return Ok();
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }
}
