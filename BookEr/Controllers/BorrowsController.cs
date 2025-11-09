using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookEr.Persistence;
using BookEr.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookEr.Web.Controllers
{
    [Authorize(Roles = "Visitor")]
    public class BorrowsController : Controller
    {
        private readonly IBookerService _service;

        private readonly UserManager<ApplicationUser> _userManager;

        public BorrowsController(IBookerService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {

            BorrowViewModel? Borrow = _service.NewBorrow(id);

            if (Borrow == null)
                return NotFound();

            if (Borrow.Volume == null)
                return NotFound();


            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                
                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                
                if (user != null)
                {
                    Visitor? visitor = _service.FindVisitorByUserId(user.Id);
                    if(visitor == null) 
                    {
                        return NotFound();
                    }
                    Borrow.VisitorAddress = visitor.Address;
                    Borrow.VisitorEmail = visitor.ApplicationUser.Email;
                    Borrow.VisitorName = visitor.ApplicationUser.Name;
                    Borrow.VisitorPhoneNumber = visitor.ApplicationUser.PhoneNumber;
                }

            }

            return View(Borrow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Int32 libraryId, BorrowViewModel Borrow)
        {
            Borrow.Volume = _service.GetVolumeById(libraryId);

            if (Borrow.Volume == null)
                return RedirectToAction("Index", "Home");

            switch (_service.ValidateBorrow(Borrow.BorrowStartDate, Borrow.BorrowEndDate, libraryId))
            {
                case BorrowDateError.StartInvalid:
                    ModelState.AddModelError("BorrowStartDate",
                        "The start date is invalid.");
                    break;
                case BorrowDateError.EndInvalid:
                    ModelState.AddModelError("BorrowEndDate",
                        "The end date is invalid.");
                    break;
                case BorrowDateError.Conflict:
                    ModelState.AddModelError("BorrowStartDate", "The given date is conflicting with an existing borrow.");
                    break;
            }

            if (!ModelState.IsValid)
                return View("Index", Borrow);

            
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (!_service.SaveBorrowAsync(libraryId, user.Id, Borrow))
                {
                    ModelState.AddModelError("", "Register new Borrow was unsuccesfull, please try again!");
                    return View("Index", Borrow);
                }

            }

            ViewBag.Message = "Successful";
            return View("Result", Borrow);
        }


        public async Task<IActionResult> VisitorBorrows()
        {
			if (User.Identity != null && User.Identity.IsAuthenticated)
            {
				ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                if(user != null)
                {
					return View("VisitorBorrows", _service.GetBorrowsForVisitor(user.Id));
				}
			}

            return Unauthorized();
		}


    }
}
