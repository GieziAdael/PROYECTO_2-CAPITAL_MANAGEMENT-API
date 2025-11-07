using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly MyAppDbContext _context;

        public EmployeeRepo(MyAppDbContext context)
        {
            _context = context;
        }
        public async Task<string> WhatThisRole(int UserId, int OrgId)
        {
            var register = await _context.Employees.FirstOrDefaultAsync(u => u.UserId == UserId && u.OrganizationId == OrgId);
            if (register == null)
            {
                return "";
            }
            return register.Role;
        }
        public async Task<bool> ExistsByEmailToOrg(int orgId, string email)
        {
            var emailN = email.ToLower().Trim();
            var emailId = await _context.Users.FirstOrDefaultAsync(u => u.EmailNormalized.Trim().ToLower() == emailN);
            if(emailId == null) return false;
            return await _context.Employees.AnyAsync(u => u.OrganizationId == orgId && u.UserId == emailId.Id);
        }
        public async Task<Employee> GetByIdUserEmployee(int id)
        {
            var registro = await _context.Employees.FirstOrDefaultAsync(u =>u.UserId == id);
            if (registro == null) return null;
            return registro;
        }

        public async Task<User> GetByIdUserEmployee(string email)
        {
            var emailN = email.ToLower().Trim();
            var  register = await _context.Users.FirstOrDefaultAsync
                (u => u.EmailNormalized.Trim().ToLower() == emailN);
            if(register == null) return null;
            return register;
        }
        public async Task<bool> NewEmployeeToOrg(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PatchRoleEmployeeToOrg(Employee employee)
        {
            _context.Employees.UpdateRange(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteEmployeeToOrg(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsMember(int UserId, int OrgId)
        {
            return await _context.Employees.AnyAsync(u => u.UserId == UserId && u.OrganizationId == OrgId);
        }

        public async Task<IEnumerable<Employee>> GetMembersByOrganizationId(int OrgId)
        {
            return await _context.Employees.Include(e => e.User)
                .Where(e => e.OrganizationId == OrgId)
                .ToListAsync();
        }

        public async Task<Employee> BodyByEmailToOrg(int orgId, string email)
        {
            var emailN = email.ToLower().Trim();
            var emailId = await _context.Users.FirstOrDefaultAsync(u => u.EmailNormalized.Trim().ToLower() == emailN);
            if (emailId == null) return null;
            var register = await _context.Employees.FirstOrDefaultAsync(u => u.OrganizationId == orgId && u.UserId == emailId.Id);
            if (register == null)
                return null;
            return register;
        }
    }
}
