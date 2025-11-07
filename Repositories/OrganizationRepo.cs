using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    public class OrganizationRepo : IOrganizationRepo
    {
        private readonly MyAppDbContext _context;

        public OrganizationRepo(MyAppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsOrgByName(string name)
        {
            var nameN = name.ToLower().Trim();
            return await _context.Organizations.AnyAsync(u => u.NameOrg.ToLower().Trim() == nameN);
        }
        public async Task<bool> ExistsOrgById(int id)
        {
            return await _context.Organizations.AnyAsync(u => u.Id == id);
        }
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
        public async Task<bool> NewOrganitation(Organization organization)
        {
            organization.PasswordOrgHash = BCrypt.Net.BCrypt.HashPassword(organization.PasswordOrgHash);
            await _context.Organizations.AddAsync(organization);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PatchOrg(Organization organization)
        {
            organization.PasswordOrgHash = BCrypt.Net.BCrypt.HashPassword(organization.PasswordOrgHash);
            _context.Organizations.Update(organization);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteOrg(Organization organization)
        {
            _context.Organizations.Remove(organization);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Organization>> GetAllMyOrginations(int id)
        {
            return await _context.Organizations.Where(c => c.UserId == id).ToListAsync();
        }
        public async Task<Organization> GetOrganizationById(int id)
        {
            var registro = await _context.Organizations.FirstOrDefaultAsync(u => u.Id == id);
            if(registro == null)
            {
                return null;
            }
            return registro;
        }
    }
}
