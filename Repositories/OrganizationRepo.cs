using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    /// <summary>
    /// Repository for managing Organization entities.
    /// </summary>
    public class OrganizationRepo : IOrganizationRepo
    {
        private readonly MyAppDbContext _context;

        /// <summary>
        /// Constructor for OrganizationRepo
        /// </summary>
        /// <param name="context"></param>
        public OrganizationRepo(MyAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Method to check if an organization exists by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> ExistsOrgByName(string name)
        {
            var nameN = name.ToLower().Trim();
            return await _context.Organizations.AnyAsync(u => u.NameOrg.ToLower().Trim() == nameN);
        }
        /// <summary>
        /// Method to check if an organization exists by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ExistsOrgById(int id)
        {
            return await _context.Organizations.AnyAsync(u => u.Id == id);
        }
        /// <summary>
        /// Method to get an organization by name and password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Organization> GetOrganization(string name, string password)
        {
            var registro = await _context.Organizations.FirstOrDefaultAsync
                (u => u.NameOrg == name);

            if (registro == null)
                return null;

            bool passwordValida = BCrypt.Net.BCrypt.Verify(password, registro.PasswordOrgHash);

            if (!passwordValida)
                return null;

            return registro;
        }
        /// <summary>
        /// Method to create a new organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public async Task<bool> NewOrganitation(Organization organization)
        {
            organization.PasswordOrgHash = BCrypt.Net.BCrypt.HashPassword(organization.PasswordOrgHash);
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to update an organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public async Task<bool> PatchOrg(Organization organization)
        {
            organization.PasswordOrgHash = BCrypt.Net.BCrypt.HashPassword(organization.PasswordOrgHash);
            _context.Organizations.Update(organization);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to delete an organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public async Task<bool> DeleteOrg(Organization organization)
        {
            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to get all organizations associated with a user ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> GetAllMyOrginations(int id)
        {
            return await _context.Organizations.Include(c => c.Employees).Where(c => c.UserId == id).ToListAsync();
        }
        /// <summary>
        /// Method to get an organization by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Organization> GetOrganizationById(int id)
        {
            var registro = await _context.Organizations.FirstOrDefaultAsync(c => c.Id == id);
            if(registro == null)
            {
                return null;
            }
            return registro;
        }

        /// <summary>
        /// Return list of organizations I am affiliated with
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Organization>> GetAllOthersOrganizationsByOrgId(int TokenId)
        {
            return await _context.Organizations
                .Include(c=>c.Employees)
                .Where(c => 
                    c.Employees.Any(e => e.UserId == TokenId) && //Im Employee
                    c.UserId != TokenId //But, im not owner
                )
                .ToListAsync();
        }
    }
}
