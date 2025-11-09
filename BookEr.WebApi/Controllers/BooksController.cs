using Microsoft.AspNetCore.Mvc;
using BookEr.Web.Models;
using BookEr.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using BookEr.Persistence;

namespace BookEr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Librarian")]
    public class BooksController : ControllerBase
    {
        private readonly IBookerService _service;
        private readonly IMapper _mapper;

        public BooksController(IBookerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetBooks()
        {
            return _service
                .GetBooks()
                .Select(book => _mapper.Map<BookDto>(book))
                .ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<BookDto> GetBookBy(Int32 id)
        {
            try
            {
                var book = _service.GetBookById(id);
                return _mapper.Map<BookDto>(book);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public ActionResult<BookDto> PostBook(BookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            var newBook = _service.AddBook(book);

            if (newBook is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                return CreatedAtAction(nameof(GetBookBy), new { id = book.Id },
                    _mapper.Map<BookDto>(book));
            }
        }
    }
}
