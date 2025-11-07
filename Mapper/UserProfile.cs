using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User,UserDto>().ReverseMap();
            CreateMap<User, FB_CreateUserDto>().ReverseMap();
        }
    }
}
