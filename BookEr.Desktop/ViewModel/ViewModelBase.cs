using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookEr.Desktop.ViewModel
{

    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;


        public event EventHandler<MessageEventArgs>? MessageApplication;

        protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnMessageApplication(String message)
        {
            MessageApplication?.Invoke(this, new MessageEventArgs(message));
        }
    }
}
