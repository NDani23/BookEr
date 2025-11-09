using BookEr.Desktop.View;
using BookEr.Desktop.ViewModel;
using BookEr.Desktop.Model;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;

namespace BookEr.Desktop
{

    public partial class App : Application
    {
        private BookErApiService _service;
        private MainViewModel _mainViewModel;
        private LoginViewModel _loginViewModel;
        private RegisterViewModel _registerViewModel;
        private MainWindow _mainView;
        private LoginWindow _loginView;
        private AddBookWindow _addBookView;
        private BorrowWindow _borrowView;
        private RegisterWindow _registerView;

        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _service = new BookErApiService(ConfigurationManager.AppSettings["baseAddress"]);

            _loginViewModel = new LoginViewModel(_service);

            _loginViewModel.LoginSucceeded += ViewModel_LoginSucceeded;
            _loginViewModel.LoginFailed += ViewModel_LoginFailed;
            _loginViewModel.StartRegister += ViewModel_StartRegister;
            _loginViewModel.MessageApplication += ViewModel_MessageApplication;

            _loginView = new LoginWindow
            {
                DataContext = _loginViewModel
            };

            _registerViewModel = new RegisterViewModel(_service);
            _registerViewModel.MessageApplication += ViewModel_MessageApplication;
            _registerViewModel.RegisterSucceeded += ViewModel_RegisterSucceeded;

            _mainViewModel = new MainViewModel(_service);
            _mainViewModel.LogoutSucceeded += ViewModel_LogoutSucceeded;
            _mainViewModel.MessageApplication += ViewModel_MessageApplication;
            _mainViewModel.StartingAddBook += ViewModel_StartingAddBook;
            _mainViewModel.FinishingAddBook += ViewModel_FinishingAddBook;
            _mainViewModel.StartingImageChange += ViewModel_StartingImageChange;
            _mainViewModel.ShowBorrows += ViewModel_ShowBorrowWindow;

            _mainView = new MainWindow
            {
                DataContext = _mainViewModel
            };


            MainWindow = _mainView;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            _loginView.Closed += LoginView_Closed;

            _loginView.Show();
        }

        private void ViewModel_MessageApplication(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message, "BookEr", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void LoginView_Closed(object sender, EventArgs e)
        {
            Shutdown();
        }

        private void ViewModel_LoginSucceeded(object sender, EventArgs e)
        {
            _loginView.Hide();
            _mainView.Show();
        }

        private void ViewModel_RegisterSucceeded(object sender, EventArgs e)
        {
            _registerView.Hide();
            _mainView.Show();
        }

        private void ViewModel_StartRegister(object sender, EventArgs e)
        {
            _loginView.Hide();
            _registerView = new RegisterWindow()
            {
                DataContext = _registerViewModel
            };
            _registerView.ShowDialog();
        }

        private void ViewModel_LoginFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Login unsuccessful!", "BookEr", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void ViewModel_LogoutSucceeded(object sender, EventArgs e)
        {
            _mainView.Hide();
            _loginView.Show();
        }

        private void ViewModel_StartingAddBook(object sender, EventArgs e)
        {
            _addBookView = new AddBookWindow
            {
                DataContext = _mainViewModel
            };
            _addBookView.ShowDialog();
        }

        private void ViewModel_FinishingAddBook(object sender, EventArgs e)
        {
            if (_addBookView.IsActive)
            {
                _addBookView.Close();
            }
        }

        private async void ViewModel_StartingImageChange(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Images|*.jpg;*.jpeg;*.bmp;*.tif;*.gif;*.png;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (dialog.ShowDialog(_addBookView).GetValueOrDefault(false))
            {
                if(_mainViewModel.NewBook != null)
                {
                    _mainViewModel.NewBook.Image = await File.ReadAllBytesAsync(dialog.FileName);
                }
                
            }
        }

        private void ViewModel_ShowBorrowWindow(object sender, EventArgs e)
        {
            if(_mainViewModel.Borrows.Count() == 0)
            {
                MessageBox.Show("No borrow registered for this volume", "BookEr", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                _borrowView = new BorrowWindow
                {
                    DataContext = _mainViewModel
                };
                _borrowView.ShowDialog();
            }
            
        }
    }
}
