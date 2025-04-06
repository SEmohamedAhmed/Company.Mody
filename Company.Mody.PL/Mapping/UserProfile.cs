using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.AppUser;
using Company.Mody.PL.DTOs.User;

namespace Company.Mody.PL.Mapping
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            
            CreateMap<AppUser, SignupViewModel>().ReverseMap();
            CreateMap<AppUser, UserViewModel>().ReverseMap();
            CreateMap<AppUser, ExternalAuthUser>().ReverseMap();

        }

    }
}
