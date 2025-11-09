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
    public class BorrowsController : ControllerBase
    {
        private readonly IBookerService _service;
        private readonly IMapper _mapper;

        public BorrowsController(IBookerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{libraryId}")]
        public ActionResult<IEnumerable<BorrowDto>> GetBorrows(int libraryId)
        {
            try
            {
                return _service
                    .GetBorrowsForPresent(libraryId)
                    .Select(borrow => _mapper.Map<BorrowDto>(borrow))
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpPut("{Id}")]
        public IActionResult PutBorrow(Int32 Id, BorrowDto borrowDto)
        {
            if (Id != borrowDto.Id)
            {
                return BadRequest();
            }

            var borrow = _mapper.Map<Borrow>(borrowDto);

            if(borrowDto.IsActive)
            {
                var borrows = _service.GetBorrowsForPresent(borrowDto.VolumeId);

                foreach (var b in borrows)
                {
                    if (b.IsActive)
                    {
                        return BadRequest();
                    }
                }
            }
            

            if (_service.UpdateBorrowActiveState(borrow))
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
