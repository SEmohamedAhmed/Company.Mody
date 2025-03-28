using AutoMapper;
using Company.Mody.DAL.Models;
using Company.Mody.PL.DTOs.AppUser;

namespace Company.Mody.PL.Mapping
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            
            CreateMap<AppUser, SignupViewModel>().ReverseMap();

        }

    }
}
