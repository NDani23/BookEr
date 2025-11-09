using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using BookEr.Desktop.Model;
using BookEr.DTO;

namespace BookEr.Desktop.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly BookErApiService _service;
        private ObservableCollection<BookViewModel>? _books;
        private VolumeViewModel? _selectedVolume;
        private BookViewModel? _selectedBook;
        private BorrowViewModel? _selectedBorrow;
        private BookViewModel? _newBook;
        private String _isItAvailable = "Borrowed";
        private String? _selectedBookTitle;
        private uint _newBookVolumeCount;

        public String IsItAvailable
        {
            get { return _isItAvailable; }
            set { _isItAvailable = value; OnPropertyChanged(); }
        }

        public ObservableCollection<BookViewModel>? Books
        {
            get { return _books; }
            set { _books = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VolumeViewModel>? _volumes;

        public ObservableCollection<VolumeViewModel>? Volumes
        {
            get { return _volumes; }
            set { _volumes = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BorrowViewModel>? _borrows;

        public ObservableCollection<BorrowViewModel>? Borrows
        {
            get { return _borrows; }
            set { _borrows = value; OnPropertyChanged(); }
        }

        public VolumeViewModel? SelectedVolume
        {
            get { return _selectedVolume; }
            set { _selectedVolume = value; OnPropertyChanged(); }
        }

        public BookViewModel? SelectedBook
        {
            get { return _selectedBook; }
            set { _selectedBook = value; OnPropertyChanged(); }
        }

        public BorrowViewModel? SelectedBorrow
        {
            get { return _selectedBorrow; }
            set { _selectedBorrow = value; OnPropertyChanged(); }
        }

        public uint NewBookVolumeCount
        {
            get { return _newBookVolumeCount; }
            set { _newBookVolumeCount = value; OnPropertyChanged(); }
        }

        public BookViewModel? NewBook
        {
            get { return _newBook; }
            set { _newBook = value; OnPropertyChanged(); }
        }

        public String? SelectedBookTitle
        {
            get { return _selectedBookTitle; }
            set { _selectedBookTitle = value; OnPropertyChanged(); }
        }

        public DelegateCommand RefreshBooksCommand { get; private set; }
        public DelegateCommand SelectCommand { get; private set; }
        public DelegateCommand VolumeSelectCommand { get; private set; }
        public DelegateCommand LogoutCommand { get; private set; }
        public DelegateCommand DeleteVolumeCommand { get; private set; }
        public DelegateCommand ChangeActiveStateCommand { get; private set; }
        public DelegateCommand AddVolumeCommand { get; private set; }
        public DelegateCommand AddBookCommand { get; private set; }
        public DelegateCommand CancelAddBookCommand { get; private set; }
        public DelegateCommand ChangeImageCommand { get; private set; }
        public DelegateCommand SaveNewBookCommand { get; private set; }
        public DelegateCommand ShowBorrowsCommand { get; private set; }

        public event EventHandler? LogoutSucceeded;
        public event EventHandler? StartingAddBook;
        public event EventHandler? FinishingAddBook;
        public event EventHandler? StartingImageChange;
        public event EventHandler? ShowBorrows;

        public MainViewModel(BookErApiService service)
        {
            _service = service;

            RefreshBooksCommand = new DelegateCommand(_ => LoadBooksAsync());
            SelectCommand = new DelegateCommand(_ => !(SelectedBook is null), param => LoadVolumesAsync(SelectedBook));
            AddBookCommand = new DelegateCommand(_ => AddBookStart());
            CancelAddBookCommand = new DelegateCommand(_ => CancelAddBook());
            VolumeSelectCommand = new DelegateCommand(_ => !(SelectedVolume is null), param => ChangeBorrowedButtonDisplayAsync(SelectedVolume));
            LogoutCommand = new DelegateCommand(_ => LogoutAsync());
            ChangeActiveStateCommand = new DelegateCommand(_ => !(SelectedBorrow is null), _ => ChangeActiveState(SelectedBorrow));
            AddVolumeCommand = new DelegateCommand(_ => !(SelectedBook is null), _ => AddVolumeToBook(SelectedBook));
            ChangeImageCommand = new DelegateCommand(_ => StartingImageChange?.Invoke(this, EventArgs.Empty));
            SaveNewBookCommand = new DelegateCommand(_ => SaveNewBook());
            ShowBorrowsCommand = new DelegateCommand(_=> !(SelectedVolume is null), _ => ShowBorrowsAsync(SelectedVolume));

            DeleteVolumeCommand = new DelegateCommand(_ => !(SelectedVolume is null), param => DeleteVolume(SelectedVolume));
        }

        private async void LoadBooksAsync()
        {
            try
            {
                Books = new ObservableCollection<BookViewModel>((await _service.LoadBooksAsync()).Select(list => (BookViewModel)list));
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void LoadVolumesAsync(BookViewModel book)
        {
            if (book is null)
            {
                Volumes = null;
                return;
            }

            SelectedBookTitle = book.Title;

            try
            {
                Volumes = new ObservableCollection<VolumeViewModel>((await _service.LoadVolumesAsync(book.Id))
                    .Select(item => (VolumeViewModel)item));
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }
        }

        private async void ShowBorrowsAsync(VolumeViewModel volume)
        {
            if (volume is null)
            {
                Borrows = null;
                return;
            }

            try
            {
                Borrows = new ObservableCollection<BorrowViewModel>((await _service.LoadBorrowsAsync(volume.LibraryId))
                    .Select(item => (BorrowViewModel)item));
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occured! ({ex.Message})");
            }

            ShowBorrows?.Invoke(this, EventArgs.Empty);
        }

        private async void LogoutAsync()
        {
            try
            {
                await _service.LogoutAsync();
                LogoutSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occurred! ({ex.Message})");
            }

        }

        private void AddBookStart()
        {
            NewBook = new BookViewModel();
            StartingAddBook?.Invoke(this, EventArgs.Empty);
        }

        private void CancelAddBook()
        {
            NewBook = null;
            FinishingAddBook?.Invoke(this, EventArgs.Empty);
        }

        private async void DeleteVolume(VolumeViewModel volume)
        {
            try
            {
                bool bookDeleted = await _service.DeleteVolumeAsync(volume.LibraryId);

                if (Volumes != null && SelectedVolume != null && Books != null && SelectedBook != null)
                {
                    Volumes.Remove(SelectedVolume);
                    SelectedVolume = null;

                    if (bookDeleted)
                    {
                        Books.Remove(SelectedBook);
                        SelectedBook = null;
                        LoadBooksAsync();
                    }
                }
                
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
               if(ex.Message.Contains("NotAllowed"))
               {
                    OnMessageApplication("Volumes having active borrow can not be deleted!");
               }
               else
               {
                    OnMessageApplication($"Unexpected error occurred! ({ex.Message})");
               }
                
            }
        }

        public async void ChangeActiveState(BorrowViewModel borrow)
        {
            try
            {
                await _service.ChangeBorrowActiveStateAsync((BorrowDto)borrow);

                if(Borrows != null)
                {
                    int borrowIndex = Borrows.IndexOf(borrow);
                    if (borrowIndex >= 0)
                    {
                        Borrows.ElementAt(borrowIndex).IsActive = Borrows.ElementAt(borrowIndex).IsActive ? false : true;
                    }

                    if (SelectedBook != null)
                    {
                        LoadVolumesAsync(SelectedBook);
                    }
                }               
                
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
              
               OnMessageApplication($"Only 1 Borrow can be active at a time! ({ex.Message})");
                
            }
        }

        public void ChangeBorrowedButtonDisplayAsync(VolumeViewModel volume)
        {
            IsItAvailable = volume.IsAvailable ? "Borrowed" : "Returned";
        }

        public async void AddVolumeToBook(BookViewModel book)
        {
            try
            {
                await _service.AddVolumeToBookAsync((BookDto)book);                
                LoadVolumesAsync(book);                
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {

                OnMessageApplication($"Unexpected error occurred! ({ex.Message})");

            }
        }

        private async void SaveNewBook()
        {
            try
            {
                if(NewBookVolumeCount <= 0)
                {
                    OnMessageApplication($"Incorrect volume count!");
                    return;
                };

                if (NewBook == null)
                {
                    OnMessageApplication($"Something went wrong!");
                    return;
                }

                if(String.IsNullOrEmpty(NewBook.Author))
                {
                    OnMessageApplication($"Incorrect author");
                    return;
                }

                if (String.IsNullOrEmpty(NewBook.Title))
                {
                    OnMessageApplication($"Incorrect title");
                    return;
                }

                if (String.IsNullOrEmpty(NewBook.ISBN))
                {
                    OnMessageApplication($"Incorrect ISBN");
                    return;
                }

                if(NewBook.Year == 0)
                {
                    OnMessageApplication($"Incorrect year");
                    return;
                }

                BookDto bookDto = await _service.AddBookAsync((BookDto)NewBook, NewBookVolumeCount);

                if(bookDto != null)
                {
                    for(int i = 0; i< NewBookVolumeCount; i++)
                    {
                        AddVolumeToBook((BookViewModel)bookDto);
                    }
                }
                
                
            }
            catch (Exception ex) when (ex is NetworkException || ex is HttpRequestException)
            {
                OnMessageApplication($"Unexpected error occurred! ({ex.Message})");
            }
            FinishingAddBook?.Invoke(this, EventArgs.Empty);
            LoadBooksAsync();
        }
    }
}
