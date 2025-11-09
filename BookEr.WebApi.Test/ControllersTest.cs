using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEr.Persistence;
using BookEr.Web.Models;
using BookEr.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using BookEr.WebApi.MappingConfigurations;
using Xunit;
using BookEr.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;
using NuGet.Protocol;

namespace BookEr.WebApi.Test
{
    public class ControllersTest : IDisposable
    {
        private readonly BookerDbContext _context;
        private readonly BookerService _service;
        private readonly BooksController _booksController;
        private readonly BorrowsController _borrowsController;
        private readonly VolumesController _volumesController;
        private readonly IMapper _mapper;
        private readonly string imgDir = "../BookEr.Persistence/App_Data";

        public ControllersTest()
        {
            var options = new DbContextOptionsBuilder<BookerDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new BookerDbContext(options);
            TestDbInitializer.Initialize(_context, imgDir);

            _context.ChangeTracker.Clear();
            _service = new BookerService(_context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BookProfile());
                cfg.AddProfile(new BookDtoProfile());
                cfg.AddProfile(new VolumeProfile());
                cfg.AddProfile(new VolumeDtoProfile());
                cfg.AddProfile(new BorrowProfile());
                cfg.AddProfile(new BorrowDtoProfile());
            });
            _mapper = new Mapper(config);
            _booksController = new BooksController(_service, _mapper);
            _borrowsController = new BorrowsController(_service, _mapper);
            _volumesController = new VolumesController(_service, _mapper);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void GetBooksTest()
        {
            // Act
            var result = _booksController.GetBooks();

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<BookDto>>(result.Value);
            Assert.Equal(22, content.Count());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void GetBookByIdTest(Int32 id)
        {
            // Act
            var result = _booksController.GetBookBy(id);

            // Assert
            var content = Assert.IsAssignableFrom<BookDto>(result.Value);
            Assert.Equal(id, content.Id);
        }

        [Fact]
        public void GetInvalidBookTest()
        {
            // Arrange
            var id = 500;

            // Act
            var result = _booksController.GetBookBy(id);

            // Assert
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public void PostBookTest()
        {
            string testTitle = "TestBook";
            string testAuthor = "TestAuthor";
            BookDto newBook = new BookDto
            {
                Id = 50,
                Title = "TestBook",
                Author = "TestAuthor",
                Year = 2023,
                ISBN = "123456789",
                Image = null
            };
            // Act
            var result = _booksController.PostBook(newBook);

            // Assert
            var postedBook = _booksController.GetBookBy(50);
            Assert.Equal(postedBook?.Value?.Title, testTitle);
            Assert.Equal(postedBook?.Value?.Author, testAuthor);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(22)]
        public void GetBorrowsTest(int bookId)
        {
            // Act
            var result = _borrowsController.GetBorrows(bookId);

            int expectedCount = 0;

            switch (bookId)
            {
                case 1:
                    expectedCount = 0;
                    break;
                case 2:
                    expectedCount = 2;
                    break;
            }

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<BorrowDto>>(result.Value);
            Assert.Equal(expectedCount, content.Count());
        }

        [Fact]
        public void GetBorrowsInvalidTest()
        {
            var result = _borrowsController.GetBorrows(1000);
            if (result.Value != null)
            {
                Assert.Empty(result.Value);
            }
        }

        [Fact]
        public void ChangeBorrowActiveStateTest()
        {
            var borrows = _borrowsController.GetBorrows(22);
            var volume = _volumesController.GetVolume(53);

            BorrowDto? borrow = null;
            if (borrows.Value != null)
            {
                borrow = borrows.Value.FirstOrDefault();
            }

            if (borrow != null && volume.Value != null)
            {
                Assert.True(borrow.IsActive);

                Assert.False(volume.Value.IsAvailable);

                borrow.IsActive = false;


                var requestResult = _borrowsController.PutBorrow(borrow.Id, borrow);

                Assert.IsAssignableFrom<OkResult>(requestResult);

                volume = _volumesController.GetVolume(53);
                if (volume.Value != null)
                {
                    Assert.True(volume.Value.IsAvailable);
                }


                borrows = _borrowsController.GetBorrows(22);
                if (borrows.Value != null)
                {
                    borrow = borrows.Value.First();
                }
                Assert.False(borrow.IsActive);

            }


        }

        [Fact]
        public void ChangeBorrowActiveStateInvalidTest()
        {
            var borrows = _borrowsController.GetBorrows(22);
            var volume = _volumesController.GetVolume(53);

            BorrowDto? borrow = null;
            if (borrows.Value != null)
            {
                borrow = borrows.Value.Where(b => b.IsActive == false).FirstOrDefault();
            }

            if (borrow != null && volume.Value != null)
            {
                Assert.False(borrow.IsActive);

                Assert.False(volume.Value.IsAvailable);

                borrow.IsActive = true;


                var requestResult = _borrowsController.PutBorrow(borrow.Id, borrow);

                Assert.IsAssignableFrom<BadRequestResult>(requestResult);

                volume = _volumesController.GetVolume(53);
                if (volume.Value != null)
                {
                    Assert.False(volume.Value.IsAvailable);
                }


                borrows = _borrowsController.GetBorrows(22);
                if (borrows.Value != null)
                {
                    borrow = borrows.Value.Where(b => b.IsActive == false).FirstOrDefault();
                }
                Assert.False(borrow.IsActive);

            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GetVolumesTest(int bookId)
        {
            // Act
            var result = _volumesController.GetVolumes(bookId);

            int expectedCount = 0;

            switch (bookId)
            {
                case 1:
                    expectedCount = 1;
                    break;
                case 2:
                    expectedCount = 4;
                    break;
                case 3:
                    expectedCount = 4;
                    break;
                case 4:
                    expectedCount = 2;
                    break;
            }

            // Assert
            var content = Assert.IsAssignableFrom<IEnumerable<VolumeDto>>(result.Value);
            Assert.Equal(expectedCount, content.Count());
        }


        [Fact]
        public void GetVolumesInvalidTest()
        {
            var result = _volumesController.GetVolumes(121);
            if (result.Value != null)
            {
                Assert.Empty(result.Value);
            }

        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public void GetVolumesById(Int32 id)
        {
            // Act
            var result = _volumesController.GetVolume(id);

            // Assert
            var content = Assert.IsAssignableFrom<VolumeDto>(result.Value);
            Assert.Equal(id, content.LibraryId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void DeleteVolumeTest(Int32 id)
        {
            var result = _volumesController.GetVolume(id);
            var content = Assert.IsAssignableFrom<VolumeDto>(result.Value);

            var deleteResult = _volumesController.DeleteVolume(id);
            Assert.IsAssignableFrom<OkResult>(deleteResult);

            result = _volumesController.GetVolume(id);
            Assert.IsAssignableFrom<NotFoundResult>(result.Result);
        }

        [Fact]
        public void DeleteBookTest()
        {
            var result = _volumesController.GetVolume(1);
            if(result.Value != null)
            {
                Assert.Equal(1, result.Value.BookId);
            }

            var deletableBook = _booksController.GetBookBy(1);
            Assert.IsAssignableFrom<BookDto>(deletableBook.Value);

            _volumesController.DeleteVolume(1);
            deletableBook = _booksController.GetBookBy(1);
            Assert.IsAssignableFrom<NotFoundResult>(deletableBook.Result);

        }

        [Fact]
        public void DeleteVolumeInvalidTest()
        {

            var deleteResult = _volumesController.DeleteVolume(53);
            Assert.IsAssignableFrom<StatusCodeResult>(deleteResult);
        }

        [Fact]
        public void PutVolumeTest()
        {
            VolumeDto modifiedVolume = new VolumeDto
            {
               LibraryId = 53,
               BookId = 22,
               IsAvailable = true,
            };


            var putResult = _volumesController.PutItem(53,modifiedVolume);
            Assert.IsAssignableFrom<OkResult>(putResult);

            var result = _volumesController.GetVolume(53);
            if(result.Value != null)
            {
                Assert.True(result.Value.IsAvailable);
            }
        }

        [Fact]
        public void PostVolumeTest()
        {
            var book = _booksController.GetBookBy(1);
            int volumeCountBefore = 0;

            if (book.Value != null)
            {
                var bookId = book.Value.Id;
                volumeCountBefore = _volumesController.GetVolumes(bookId).Value.Count();
                var postResult = _volumesController.PostVolume(book.Value);
                Assert.IsAssignableFrom<CreatedAtActionResult>(postResult.Result);
            }

            book = _booksController.GetBookBy(1);
            if (book.Value != null)
            {
                var bookId = book.Value.Id;
                var volumeCountAfter = _volumesController.GetVolumes(bookId).Value.Count();
                Assert.Equal(volumeCountBefore + 1, volumeCountAfter);              
            }
        }
    }
}
