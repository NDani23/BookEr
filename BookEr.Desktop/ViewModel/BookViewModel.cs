using System;
using BookEr.DTO;

namespace BookEr.Desktop.ViewModel
{
    public class BookViewModel : ViewModelBase
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private String _title = null!;

        public String Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        private String _author = null!;

        public String Author
        {
            get { return _author; }
            set { _author = value; OnPropertyChanged(); }
        }

        private Int32 _year;

        public Int32 Year
        {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }

        private String _isbn = null!;

        public String ISBN
        {
            get { return _isbn; }
            set { _isbn = value; OnPropertyChanged(); }
        }

        private Byte[] _image = null!;

        public Byte[] Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        public static explicit operator BookViewModel(BookDto dto) => new BookViewModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Author = dto.Author,
            Year = dto.Year,
            ISBN = dto.ISBN,
            Image = dto.Image
        };

        public static explicit operator BookDto(BookViewModel vm) => new BookDto
        {
            Id = vm.Id,
            Title = vm.Title,
            Author = vm.Author,
            Year = vm.Year,
            ISBN = vm.ISBN,
            Image = vm.Image
        };
    }
}
