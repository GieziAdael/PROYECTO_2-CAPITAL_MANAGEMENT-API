using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    public interface IOrganizationRepo
    {
        Task<bool> ExistsOrgByName(string name);
        Task<bool> ExistsOrgById(int id);
        Task<Organization> GetOrganization(string name, string password);
        Task<bool> NewOrganitation(Organization organization);
        Task<bool> PatchOrg(Organization organization);
        Task<bool> DeleteOrg(Organization organization);

        Task<IEnumerable<Organization>> GetAllMyOrginations(int id);
        Task<Organization> GetOrganizationById(int id);
    }
}
