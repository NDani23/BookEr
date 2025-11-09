using AutoMapper;
using System.Collections.Generic;
using BookEr.Persistence;
using BookEr.DTO;
using BookEr.Web.Models;

namespace BookEr.WebApi.MappingConfigurations
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>();
        }
    }

    public class BookDtoProfile : Profile
    {
        public BookDtoProfile()
        {
            CreateMap<BookDto, Book>();
        }
    }

    public class VolumeProfile : Profile
    {
        public VolumeProfile()
        {
            CreateMap<Volume, VolumeDto>()
                .ForMember(dto => dto.CurrentUser, m => m.MapFrom(
                    volume => (volume.Borrows.FirstOrDefault(b => b.IsActive) == null) ? null : volume.Borrows.FirstOrDefault(b => b.IsActive).Visitor.ApplicationUser.UserName
                ))
                .ForMember(dto => dto.CurrentDeadline, m => m.MapFrom(
                    volume => (volume.Borrows.FirstOrDefault(b => b.IsActive) == null) ? null : volume.Borrows.FirstOrDefault(b => b.IsActive).EndDay.ToString()
                ))
                .ForMember(dto => dto.NextBorrow, m => m.MapFrom(
                    volume => (volume.Borrows.FirstOrDefault(b => b.StartDay >= DateTime.Today) == null) ? null : volume.Borrows.FirstOrDefault(b => b.StartDay >= DateTime.Today).StartDay.ToString()
                ));
        }
    }

    public class VolumeDtoProfile : Profile
    {
        public VolumeDtoProfile()
        {
            CreateMap<VolumeDto, Volume>();
        }
    }

    public class BorrowProfile : Profile
    {
        public BorrowProfile()
        {
            CreateMap<Borrow, BorrowDto>().ForMember(dto => dto.UserName, m => m.MapFrom(borrow => borrow.Visitor.ApplicationUser.UserName));
        }
    }

    public class BorrowDtoProfile : Profile
    {
        public BorrowDtoProfile()
        {
            CreateMap<BorrowDto, Borrow>();
        }
    }
}
