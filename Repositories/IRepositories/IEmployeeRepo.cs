using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    /// <summary>
    /// Interface for Employee Repository
    /// </summary>
    public interface IEmployeeRepo
    {
        /// <summary>
        /// Method to get the role of a user in a specific organization
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<string> WhatThisRole(int UserId, int OrgId);
        /// <summary>
        /// Method to check if an employee exists in an organization by email
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> ExistsByEmailToOrg(int orgId, string email);
        /// <summary>
        /// Method to get an employee by user ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Employee> GetByIdUserEmployee(int id);
        /// <summary>
        /// Method to get a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetByIdUserEmployee(string email);
        /// <summary>
        /// Method to add a new employee to an organization
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task<bool> NewEmployeeToOrg(Employee employee);
        /// <summary>
        /// Method to update the role of an employee in an organization
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task<bool> PatchRoleEmployeeToOrg(Employee employee);
        /// <summary>
        /// Method to delete an employee from an organization
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task<bool> DeleteEmployeeToOrg(Employee employee);

        /// <summary>
        /// Method to check if a user is a member of an organization
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<bool> IsMember(int UserId, int OrgId);
        /// <summary>
        /// Method to get all members of an organization by organization ID
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<IEnumerable<Employee>> GetMembersByOrganizationId(int OrgId);
        /// <summary>
        /// Method to get an employee by email in a specific organization
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Employee> BodyByEmailToOrg(int orgId, string email);
    }
}
