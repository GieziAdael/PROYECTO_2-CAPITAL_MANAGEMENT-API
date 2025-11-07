using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    /// <summary>
    /// Profile for mapping between User and UserDto as well as FB_CreateUserDto.
    /// </summary>
    public class UserProfile : Profile
    {
        /// <summary>
        /// Constructor to create mapping configurations.
        /// </summary>
        public UserProfile()
        {
            CreateMap<User,UserDto>().ReverseMap();
            CreateMap<User, FB_CreateUserDto>().ReverseMap();
        }
    }
}
