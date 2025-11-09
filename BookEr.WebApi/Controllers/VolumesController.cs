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
    public class VolumesController : ControllerBase
    {
        private readonly IBookerService _service;
        private readonly IMapper _mapper;

        public VolumesController(IBookerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("{bookId}")]
        public ActionResult<IEnumerable<VolumeDto>> GetVolumes(int bookId)
        {
            try
            {
                return _service
                    .GetVolumesByBook(bookId)
                    .Select(volume => _mapper.Map<VolumeDto>(volume))
                    .ToList();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("Single/{libraryId}")]
        public ActionResult<VolumeDto> GetVolume(Int32 libraryId)
        {
            try
            {
                var volume = _service.GetVolumeById(libraryId);
                return _mapper.Map<VolumeDto>(volume);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{libraryId}")]
        public IActionResult DeleteVolume(int libraryId)
        {

            Volume? volume = _service.GetVolumeById(libraryId);

            if (volume == null)
            {
                return NotFound();
            }
            else if(!volume.IsAvailable)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }


            if (_service.DeleteVolume(libraryId))
            {
                if(volume.Book.Volumes.Count==0)
                {
                    _service.DeleteBook(volume.BookId);
                    return Ok();
                }
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("{libraryId}")]
        public IActionResult PutItem(Int32 libraryId, VolumeDto volumeDto)
        {
            if (libraryId != volumeDto.LibraryId)
            {
                return BadRequest();
            }

            var volume = _mapper.Map<Volume>(volumeDto);

            if (_service.UpdateVolume(volume))
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult<VolumeDto> PostVolume(BookDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            var newVolume = _service.AddVolume(book.Id);

            if (newVolume is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                return CreatedAtAction(nameof(GetVolume), new { libraryId = newVolume.LibraryId },
                    _mapper.Map<VolumeDto>(newVolume));

            }
        }
    }
}
