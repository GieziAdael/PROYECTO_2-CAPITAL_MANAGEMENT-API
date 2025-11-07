using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, FB_CreateOrgDto>().ReverseMap();
        }
    }
}
