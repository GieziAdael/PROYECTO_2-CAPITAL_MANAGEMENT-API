using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    public interface IEmployeeRepo
    {
        Task<string> WhatThisRole(int UserId, int OrgId);
        Task<bool> ExistsByEmailToOrg(int orgId, string email);
        Task<Employee> GetByIdUserEmployee(int id);
        Task<User> GetByIdUserEmployee(string email);
        Task<bool> NewEmployeeToOrg(Employee employee);
        Task<bool> PatchRoleEmployeeToOrg(Employee employee);
        Task<bool> DeleteEmployeeToOrg(Employee employee);

        Task<bool> IsMember(int UserId, int OrgId);
        Task<IEnumerable<Employee>> GetMembersByOrganizationId(int OrgId);
        Task<Employee> BodyByEmailToOrg(int orgId, string email);
    }
}
