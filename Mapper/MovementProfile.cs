using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    public class MovementProfile : Profile
    {
        public MovementProfile()
        {
            CreateMap<Movement, FB_CreateMovDto>().ReverseMap();
        }
    }
}
