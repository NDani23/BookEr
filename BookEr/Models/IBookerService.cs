using BookEr.Persistence;

namespace BookEr.Web.Models
{
    public interface IBookerService
    {

        List<Volume> GetVolumesByBook(int? book_id);

        Volume? GetVolumeById(int? id);

        List<Volume> GetVolumes();

        List<Book> GetBooks(String? title = null);

        List<Book> GetBooksByTitleString(string? searchString);

        List<Book> GetBooksByAuthorString(string? searchString);

        Book GetBookById(int id);

        Visitor GetVisitorByName(String name);

        Visitor GetVisitorById(int id);

        Borrow GetBorrowById(int id);

        List<Borrow> GetBorrowsForVisitor(int visitorId);

        List<Borrow> GetBorrowsForPresent(int? libraryId);

        int GetBorrowCountByBook(int? bookId);

        BorrowDateError ValidateBorrow(DateTime start, DateTime end, int libraryId);

        BorrowViewModel? NewBorrow(Int32 libraryId);

        Boolean SaveBorrowAsync(Int32 libraryId, Int32 userId, BorrowViewModel Borrow);

        Volume? AddVolume(Int32 bookId);

        Book? AddBook(Book book);

        bool UpdateBorrowActiveState(Borrow borrow);

        bool DeleteBook(int bookId);

        bool DeleteVolume(int libraryId);

        bool UpdateVolume(Volume volume);

        Visitor? FindVisitorByUserId(int userId);

        Librarian? FindLibrarianByUserId(int userId);

        Visitor? AddVisitor(Visitor visitor);

        Librarian? AddLibrarian(Librarian librarian);
    }
}
