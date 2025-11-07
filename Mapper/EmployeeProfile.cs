using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    /// <summary>
    /// Profile for mapping between Employee and FB_CreateEmployeeDto.
    /// </summary>
    public class EmployeeProfile : Profile
    {
        /// <summary>
        /// Constructor to create mapping configurations.
        /// </summary>
        public EmployeeProfile()
        {
            CreateMap<Employee, FB_CreateEmployeeDto>().ReverseMap();
        }
    }
}
