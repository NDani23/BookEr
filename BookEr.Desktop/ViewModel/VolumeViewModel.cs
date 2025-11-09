using System;
using BookEr.DTO;

namespace BookEr.Desktop.ViewModel
{
    public class VolumeViewModel : ViewModelBase
    {
        private int _libraryId;

        public int LibraryId
        {
            get { return _libraryId; }
            set { _libraryId = value; OnPropertyChanged(); }
        }

        private Boolean _isAvailable;

        public Boolean IsAvailable
        {
            get { return _isAvailable; }
            set { _isAvailable = value; OnPropertyChanged(); }
        }

        private String? _currentUser = null!;

        public String? CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private DateTime? _currentDeadline;

        public DateTime? CurrentDeadline
        {
            get { return _currentDeadline; }
            set { _currentDeadline = value; OnPropertyChanged(); }
        }

        private DateTime? _nextBorrow;

        public DateTime? NextBorrow
        {
            get { return _nextBorrow; }
            set { _nextBorrow = value; OnPropertyChanged(); }
        }

        private int _bookId;

        public int BookId
        {
            get { return _bookId; }
            set { _bookId = value; OnPropertyChanged(); }
        }

        public VolumeViewModel ShallowClone()
        {
            return (VolumeViewModel)this.MemberwiseClone();
        }

        public void CopyFrom(VolumeViewModel rhs)
        {
            LibraryId = rhs.LibraryId;
            IsAvailable = rhs.IsAvailable;
            BookId = rhs.BookId;
        }

        public static explicit operator VolumeViewModel(VolumeDto dto) => new VolumeViewModel
        {
            LibraryId= dto.LibraryId,
            IsAvailable= dto.IsAvailable,
            BookId= dto.BookId,
            CurrentUser = dto.CurrentUser,
            CurrentDeadline = dto.CurrentDeadline,
            NextBorrow = dto.NextBorrow,
        };

        public static explicit operator VolumeDto(VolumeViewModel vm) => new VolumeDto
        {
           LibraryId = vm.LibraryId,
           IsAvailable= vm.IsAvailable,
           BookId= vm.BookId,
           CurrentUser= vm.CurrentUser,
           CurrentDeadline= vm.CurrentDeadline,
           NextBorrow= vm.NextBorrow,
        };

    }
}
