using Microsoft.AspNetCore.Mvc;
using BookEr.Web.Models;

namespace BookEr.Web.Controllers
{
    public class VolumesController : Controller
    {
        private readonly IBookerService _service;

        public VolumesController(IBookerService service)
        {
            _service = service;
        }

        public IActionResult Index(int? bookId = null)
        {
              if(bookId == null)
               {
                    return View(_service.GetVolumes());
               }
               
            
            return View(_service.GetVolumes());

            
        }

        public IActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                ViewBag.LibraryId = id;
                ViewBag.Title = _service.GetVolumeById(id).Book.Title;
                return View(_service.GetBorrowsForPresent(id).OrderBy(l => l.StartDay));
            }
            catch (ArgumentNullException) 
            {
                return NotFound();
            }         
        }
    }
}
