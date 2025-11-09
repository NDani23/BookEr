using System;
using BookEr.DTO;

namespace BookEr.Desktop.ViewModel
{
    public class BorrowViewModel : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        private int _volumeId;
        public int VolumeId
        {
            get { return _volumeId; }
            set { _volumeId = value; OnPropertyChanged(); }
        }

        private int _visitorId;
        public int VisitorId
        {
            get { return _visitorId; }
            set { _visitorId = value; OnPropertyChanged(); }
        }

        private String _userName = null!;
        public String UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        private DateTime _startDay;
        public DateTime StartDay
        {
            get { return _startDay; }
            set { _startDay = value; OnPropertyChanged(); }
        }

        private DateTime _endDay;
        public DateTime EndDay
        {
            get { return _endDay; }
            set { _endDay = value; OnPropertyChanged(); }
        }

        private Boolean _isActive;
        public Boolean IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }

        public static explicit operator BorrowViewModel(BorrowDto dto) => new BorrowViewModel
        {
            Id = dto.Id,
            VolumeId = dto.VolumeId,
            VisitorId = dto.VisitorId,
            StartDay = dto.StartDay,
            EndDay = dto.EndDay,
            IsActive = dto.IsActive,
            UserName = dto.UserName,
        };

        public static explicit operator BorrowDto(BorrowViewModel vm) => new BorrowDto
        {
            Id = vm.Id,
            VolumeId = vm.VolumeId,
            VisitorId = vm.VisitorId,
            StartDay = vm.StartDay,
            EndDay = vm.EndDay,
            IsActive = vm.IsActive,
            UserName = vm.UserName,
        };
    }
}
