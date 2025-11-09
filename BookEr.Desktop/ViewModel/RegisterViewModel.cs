using System;
using System.Net.Http;
using BookEr.Desktop.Model;
using BookEr.DTO;
using System.Text.RegularExpressions;

namespace BookEr.Desktop.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly BookErApiService _model;
        private Boolean _isLoading;

        public DelegateCommand RegisterCommand { get; private set; }

        public String Name { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Password { get; set; }
        public String PasswordRepeated { get; set; }

        public Boolean IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler? RegisterSucceeded;

        public event EventHandler? RegisterFailed;


        public RegisterViewModel(BookErApiService model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _model = model;
            Name = String.Empty;
            UserName = String.Empty;
            Email = String.Empty;
            PhoneNumber = String.Empty;
            Password = String.Empty;
            PasswordRepeated = String.Empty;

            IsLoading = false;

            RegisterCommand = new DelegateCommand(_ => !IsLoading, param => RegisterAsync());
        }

        private async void RegisterAsync()
        {
            if(!Vadidate())
            {
                return;
            }

            try
            {
                RegisterDto newUser = new RegisterDto
                {
                    Name = Name,
                    UserName = UserName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Password = Password,
                };
                IsLoading = true;
                ResponseDto result = await _model.RegisterAsync(newUser);
                IsLoading = false;

                if (result.Success)
                    OnRegisterSuccess();
                else
                    OnRegisterFailed(result);
            }
            catch (HttpRequestException ex)
            {
                OnMessageApplication($"{ex.Message}");
            }
            catch (NetworkException ex)
            {
                OnMessageApplication($"Error during registration: ({ex.Message})");
            }
        }

        private void OnRegisterSuccess()
        {
            RegisterSucceeded?.Invoke(this, EventArgs.Empty);
        }

        private void OnRegisterFailed(ResponseDto result)
        {
            result.ErrorMessage = result.ErrorMessage ?? "Registration was unsuccessfull";
            OnMessageApplication(result.ErrorMessage);
        }

        private bool Vadidate()
        {
            if (String.IsNullOrEmpty(Password)
               || String.IsNullOrEmpty(PasswordRepeated)
               || String.IsNullOrEmpty(UserName)
               || String.IsNullOrEmpty(Name)
               || String.IsNullOrEmpty(Email)
               || String.IsNullOrEmpty(PhoneNumber))
            {
                OnMessageApplication("Some fields are empty!");
                return false;
            }

            if (Password != PasswordRepeated)
            {
                OnMessageApplication($"Mismatch in passwords!");
                return false;
            }

            string pattern = "^([\\+]?(?:00)?[0-9]{1,3}[\\s.-]?[0-9]{1,12})([\\s.-]?[0-9]{1,4}?)$";
            if (!Regex.Match(PhoneNumber, pattern, RegexOptions.IgnoreCase).Success)
            {
                OnMessageApplication($"Not a valid phone number!");
                return false;
            }

            pattern = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";
            if (!Regex.Match(Email, pattern, RegexOptions.IgnoreCase).Success)
            {
                OnMessageApplication($"Not a valid email format!");
                return false;
            }

            return true;
        }
    }
}
