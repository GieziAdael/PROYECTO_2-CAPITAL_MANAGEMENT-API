using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Domain_Services.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOrganizationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<IEnumerable<FB_ListOrganizationsDto>> MyOrganizations(int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<IEnumerable<FB_ListOrganizationsDto>> OrganizationsAfilied(int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createOrgDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Organization> CreateOrganization(FB_CreateOrgDto createOrgDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fB_LoginOrgDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Organization> LoginOrganization(FB_CreateOrgDto fB_LoginOrgDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="newPassword"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task UpdatePasswordOrganization(int orgId, string newPassword, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task DeleteOrganization(int orgId, int tokenId);
    }
}
