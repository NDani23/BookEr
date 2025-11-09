using Microsoft.AspNetCore.Mvc;
using BookEr.Persistence;
using BookEr.Web.Models;

namespace BookEr.Web.Controllers
{

    public enum SortOrder { TITLE_DESC, TITLE_ASC, POP_DESC }
    public class BooksController : Controller
    {
        private readonly IBookerService _service;

        public BooksController(IBookerService service)
        {
            _service = service;
        }

        public IActionResult Index(int? pageNumber, string? lastSearchString, bool hasSearchString=false, SortOrder sortOrder = SortOrder.POP_DESC)
        {
            int pageSize = 20;

            ViewData["SortParam"] = sortOrder == SortOrder.TITLE_DESC ? SortOrder.TITLE_ASC : SortOrder.TITLE_DESC;
            ViewData["SortParamString"] = sortOrder == SortOrder.TITLE_DESC ? "asc" : "desc";
            ViewData["PopSortParam"] = SortOrder.POP_DESC;

            ViewBag.hasSearchString = false;


            List<Book> books = _service.GetBooks();

            string? searchString = null;

            if (hasSearchString)
            {         
                searchString = Request.Form["searchString"];

            }else if(lastSearchString != null)
            {
                searchString = lastSearchString;
            }

            if(searchString != null)
            {
                books = _service.GetBooksByTitleString(searchString)
                        .Concat(_service.GetBooksByAuthorString(searchString)).ToList();

                ViewBag.hasSearchString = true;
                ViewBag.lastSearchString = searchString;
            }


            switch (sortOrder)
            {
                case SortOrder.TITLE_ASC:
                    books = books.OrderBy(i => i.Title).ThenBy(i => _service.GetBorrowCountByBook(i.Id)).ToList();
                    break;
                case SortOrder.TITLE_DESC:
                    books = books.OrderByDescending(i => i.Title).ThenBy(i => _service.GetBorrowCountByBook(i.Id)).ToList();
                    break;
                case SortOrder.POP_DESC:
                    books = books.OrderByDescending(i => _service.GetBorrowCountByBook(i.Id)).ThenBy(i => i.Title).ToList();
                    break;
                default:
                    break;
            }

            return View(PagedList<Book>.Create(books, pageNumber ?? 1, pageSize));
        }


        public IActionResult Details(Int32 id)
        {

            try
            {
                ViewBag.BookId = id;
                return View(_service.GetBookById(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        public IActionResult? DisplayImage(int id)
        {
            var item = _service.GetBookById(id);
            if (item != null && item.Image != null)
            {
                return File(item.Image, "image/png");
            }
            return null;
        }
    }
}
