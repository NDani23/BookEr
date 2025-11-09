using Microsoft.EntityFrameworkCore;
using BookEr.Persistence;

namespace BookEr.Web.Models
{
    public class BookerService : IBookerService
    {
        private readonly BookerDbContext _context;
        private readonly BorrowDateValidator _BorrowDateValidator;

        public BookerService(BookerDbContext context)
        {
            _context = context;
            _BorrowDateValidator = new BorrowDateValidator(_context);
        }

        public List<Volume> GetVolumesByBook(int? book_id)
        {
            return _context.Volumes.Include(x => x.Book).Where(v => v.Book.Id == book_id).ToList();
        }

        public Volume? GetVolumeById(int? id)
        {          
            return _context.Volumes.Include(x => x.Book).Single(v => v.LibraryId == id);
        }

        public List<Volume> GetVolumes()
        {
            return _context.Volumes.ToList();
        }

        public List<Book> GetBooks(string? title = null)
        {
            return _context.Books
                .Include(b => b.Volumes)
                .Where(b => b.Title.Contains(title ?? ""))
                .OrderBy(l => l.Title)
                .ToList();
        }

        public List<Book> GetBooksByTitleString(string? searchString)
        {
            return _context.Books
                .Include(b => b.Volumes)
                .Where(b => b.Title.Contains(searchString ?? ""))
                .OrderBy(l => l.Title)
                .ToList();
        }

        public List<Book> GetBooksByAuthorString(string? searchString)
        {
            return _context.Books
                .Include(b => b.Volumes)
                .Where(b => b.Author.Contains(searchString ?? ""))
                .OrderBy(l => l.Author)
                .ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books.Include(b => b.Volumes).Single(l => l.Id == id);
        }

        public Visitor GetVisitorByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            else
            {
                return _context.Visitors.Single(v => v.ApplicationUser.Name == name);
            }
        }

        public Visitor GetVisitorById(int id)
        {
            return _context.Visitors.Single(v => v.ApplicationUser.Id == id);
        }

        public Borrow GetBorrowById(int id)
        {
            return _context.Borrows.Single(l => l.Id == id);
        }

        public List<Borrow> GetBorrowsForVisitor(int visitorId)
        {
            return _context.Borrows.Where(b => b.VisitorId == visitorId && b.EndDay >= DateTime.Today).ToList();
        }

        public List<Borrow> GetBorrowsForPresent(int? libraryId)
        {
            if (libraryId == null)
                throw new ArgumentNullException(nameof(libraryId));


            return _context.Volumes.Single(v => v.LibraryId == libraryId).Borrows
                .Where(l => l.StartDay >= DateTime.Today || l.IsActive)
                .ToList();
        }

        public BorrowDateError ValidateBorrow(DateTime start, DateTime end, int libraryId)
        {
            return _BorrowDateValidator.Validate(start, end, libraryId);
        }

        public BorrowViewModel? NewBorrow(Int32 libraryId)
        {
            Volume? volume = _context.Volumes.Include(v => v.Book)
                            .FirstOrDefault(v => v.LibraryId == libraryId);

            if (volume == null)
                return null;

            BorrowViewModel? Borrow = new BorrowViewModel { Volume = volume };

            Borrow.BorrowStartDate = DateTime.Today + TimeSpan.FromDays(7);
            Borrow.BorrowEndDate = Borrow.BorrowStartDate + TimeSpan.FromDays(7);

            return Borrow;
        }

        public int GetBorrowCountByBook(int? bookId)
        {
            if (bookId == null)
                return 0;


            return _context.Borrows.Count(l => l.Volume.BookId == bookId);
        }

        public Boolean SaveBorrowAsync(Int32  libraryId, Int32 userId, BorrowViewModel Borrow)
        {
            if (Borrow.Volume == null)
                return false;

            Visitor? visitor = _context.Visitors.FirstOrDefault(v => v.ApplicationUser.Id == userId);

            if(visitor == null)
                return false;

            Borrow newBorrow = new Borrow
            {
                Volume = Borrow.Volume,
                Visitor = visitor,
                StartDay = Borrow.BorrowStartDate,
                EndDay = Borrow.BorrowEndDate,
                IsActive = false

            };

            _context.Borrows.Add(newBorrow);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // mentéskor lehet hiba
                return false;
            }

            // ha idáig eljutottunk, minden sikeres volt
            return true;
        }

        public bool DeleteBook(int bookId)
        {
            var book = _context.Books.Find(bookId);
            if (book == null)
            {
                return false;
            }

            try
            {
                _context.Remove(book);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool DeleteVolume(int libraryId)
        {
            var volume = _context.Volumes.Find(libraryId);
            if (volume == null)
            {
                return false;
            }

            try
            {
                var borrows = _context.Borrows.Where(b => b.VolumeId == libraryId && b.EndDay >= DateTime.Today);

                foreach (var borrow in borrows)
                {
                    _context.Remove(borrow);
                }

                _context.Remove(volume);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool UpdateVolume(Volume volume)
        {
            try
            {
                _context.Update(volume);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public bool UpdateBorrowActiveState(Borrow borrow)
        {
            try
            {
                Borrow? oldBorrow = _context.Borrows.Where(_b => _b.Id == borrow.Id).FirstOrDefault();

                if(oldBorrow == null)
                {
                    return false;
                }

                if(oldBorrow.IsActive != borrow.IsActive)
                {
                    Volume? volume = _context.Volumes.Where(v => v.LibraryId == borrow.VolumeId).FirstOrDefault();

                    if(volume == null)
                    {
                        return false;
                    }

                    volume.IsAvailable = volume.IsAvailable ? false : true;

                    _context.Update(volume);
                }

                oldBorrow.IsActive = oldBorrow.IsActive ? false : true;
                _context.Update(oldBorrow);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public Volume? AddVolume(Int32 bookId)
        {
            try
            {
                Volume newVolume = new Volume();
                Book? book = _context.Books.Where(book => book.Id == bookId).FirstOrDefault();

                if(book == null)
                {
                    return null;
                }

                book.Volumes.Add(newVolume);
                _context.Update(book);
                
                _context.SaveChanges();
                return newVolume;
            }
            catch (DbUpdateException)
            {
                return null;
            }

        }

        public Book? AddBook(Book book)
        {
            try
            {
                _context.Books.Add(book);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return null;
            }

            return book;
        }

        public Visitor? FindVisitorByUserId(int userId)
        {
            return _context.Visitors.Where(v => v.UserId == userId).FirstOrDefault();
        }

        public Librarian? FindLibrarianByUserId(int userId)
        {
            return _context.Librarians.Where(l => l.UserId == userId).FirstOrDefault();
        }

        public Visitor? AddVisitor(Visitor visitor)
        {
            try
            {
                _context.Visitors.Add(visitor);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return null;
            }

            return visitor;
        }

        public Librarian? AddLibrarian(Librarian librarian)
        {
            try
            {
                _context.Librarians.Add(librarian);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return null;
            }

            return librarian;
        }

    }
}
