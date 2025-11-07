using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    public class MovementRepo : IMovementRepo
    {
        private readonly MyAppDbContext _context;

        public MovementRepo(MyAppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Movement>> GetMovementsByIDOrg(int id)
        {
            return await _context.Movements.Where(u => u.Id == id).ToListAsync();
        }
        public async Task<Movement> GetMovByNOrg(int id, int nmov)
        {
            var registro = await _context.Movements.FirstOrDefaultAsync(u => u.OrganizationId == id && u.NoMov == nmov);
            if (registro == null)
            {
                return null;
            }
            return registro;
        }
        public async Task<bool> NewMovement(Movement movement)
        {
            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> PatchMov(Movement movement)
        {
            _context.Movements.Update(movement);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteMov(Movement movement)
        {
            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Movement>> GetAllMovements(int OrgId)
        {
            var myMovements = await _context.Movements
                .Where(o => o.OrganizationId == OrgId).OrderBy(o => o.NoMov)
                .ToListAsync();
            return myMovements;
        }
        public async Task<IEnumerable<Movement>> GetAllMov()
        {
            return await _context.Movements.ToListAsync();
        }
    }
}
