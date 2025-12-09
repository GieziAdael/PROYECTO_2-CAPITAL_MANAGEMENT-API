using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Domain_Services.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<IEnumerable<Employee>> AllMembersAOrganization(int OrgId, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="addMemberDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Employee> AddMember(int OrgId, FB_CreateEmployeeDto addMemberDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="updateRoleDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task UpdateRoleMember(int OrgId, FB_CreateEmployeeDto updateRoleDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="email"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task RemoveMember(int OrgId, string email ,int tokenId);
    }
}
