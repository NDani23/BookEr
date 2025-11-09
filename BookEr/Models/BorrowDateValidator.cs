using BookEr.Persistence;

namespace BookEr.Web.Models
{
    public class BorrowDateValidator
    {
        private readonly BookerDbContext _context;

        public BorrowDateValidator(BookerDbContext context)
        {
            _context = context;
        }

        public BorrowDateError Validate(DateTime start, DateTime end, int libraryId)
        {
            if (start < DateTime.Today)
                return BorrowDateError.StartInvalid;

            if(end < start)
                return BorrowDateError.EndInvalid;

            Volume? volume = _context.Volumes.FirstOrDefault(v => v.LibraryId == libraryId);

            if (volume == null)
                return BorrowDateError.None;

            if (_context.Borrows.Where(l => l.Volume.LibraryId == volume.LibraryId && l.EndDay >= start)
                     .ToList()
                     .Any(l => IsConflicting(l.StartDay, l.EndDay, start, end))) 
                return BorrowDateError.Conflict;

            return BorrowDateError.None;
        }

        private static Boolean IsConflicting(DateTime start, DateTime end, DateTime startNew, DateTime endNew)
        {
            return start >= startNew && start < endNew ||
                   end >= startNew && end < endNew ||
                   start < startNew && end > endNew ||
                   start > startNew && end < endNew;
        }


    }
}
