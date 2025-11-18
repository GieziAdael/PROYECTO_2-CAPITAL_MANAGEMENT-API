using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    /// <summary>
    /// Interface for Organization Repository
    /// </summary>
    public interface IOrganizationRepo
    {
        /// <summary>
        /// Method to check if an organization exists by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> ExistsOrgByName(string name);
        /// <summary>
        /// Method to check if an organization exists by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsOrgById(int id);
        /// <summary>
        /// Method to get an organization by name and password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Organization> GetOrganization(string name, string password);
        /// <summary>
        /// Method to create a new organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        Task<bool> NewOrganitation(Organization organization);
        /// <summary>
        /// Method to update an organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        Task<bool> PatchOrg(Organization organization);
        /// <summary>
        /// Method to delete an organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        Task<bool> DeleteOrg(Organization organization);

        /// <summary>
        /// Method to get all organizations associated with a user ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> GetAllMyOrginations(int id);
        /// <summary>
        /// Method to get an organization by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Organization> GetOrganizationById(int id);


        /// <summary>
        /// Return list of organizations I am affiliated with
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<IEnumerable<Organization>> GetAllOthersOrganizationsByOrgId(int TokenId);
    }
}
