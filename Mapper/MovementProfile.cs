using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    /// <summary>
    /// Profile for mapping between Movement and FB_CreateMovDto.
    /// </summary>
    public class MovementProfile : Profile
    {
        /// <summary>
        /// Constructor to create mapping configurations.
        /// </summary>
        public MovementProfile()
        {
            CreateMap<Movement, FB_CreateMovDto>().ReverseMap();
        }
    }
}
