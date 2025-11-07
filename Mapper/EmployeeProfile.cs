using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, FB_CreateEmployeeDto>().ReverseMap();
        }
    }
}
