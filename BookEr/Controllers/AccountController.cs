using Microsoft.AspNetCore.Mvc;
using BookEr.Persistence;
using BookEr.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace BookEr.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBookerService _service;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IBookerService service)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _service = service;
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(BooksController.Index), "Books");
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if(!ModelState.IsValid)
                return View("Login", vm);

            var user = await _userManager.FindByNameAsync(vm.UserName);
            if (user == null)
            {
                 ModelState.AddModelError("", "Login was unsuccessful!");
                 return View(vm);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if(!userRoles.Contains("Visitor"))
            {
                ModelState.AddModelError("", "Login was unsuccessful!");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);

            if (!result.Succeeded)
            {                   
                 ModelState.AddModelError("", "Wrong user name or password");
                 return View("Login", vm);
            }


            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View("Register", vm);

            var AppUser = new ApplicationUser {
                UserName = vm.UserName,
                Email = vm.VisitorEmail,
                Name = vm.VisitorName,
                PhoneNumber = vm.VisitorPhoneNumber
            };

            Visitor visitor = new Visitor
            {
                ApplicationUser = AppUser,
                Address = vm.VisitorAddress,
            };
            var result = await _userManager.CreateAsync(AppUser, vm.UserPassword);
           

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View("Register", vm);
            }
            await _userManager.AddToRoleAsync(AppUser, "Visitor");
            var newVisitor = _service.AddVisitor(visitor);
            await _signInManager.SignInAsync(AppUser, false); 
           
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(BooksController.Index), "Books");
        }
    }
}
