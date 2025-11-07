using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Mapper
{
    /// <summary>
    /// Profile for mapping between Organization and FB_CreateOrgDto.
    /// </summary>
    public class OrganizationProfile : Profile
    {
        /// <summary>
        /// Constructor to create mapping configurations.
        /// </summary>
        public OrganizationProfile()
        {
            CreateMap<Organization, FB_CreateOrgDto>().ReverseMap();
            CreateMap<Organization, FB_ListOrganizationsDto>().ForMember(dest => dest.NameOrganization, opt => opt.MapFrom(src => src.NameOrg));
        }
    }
}
