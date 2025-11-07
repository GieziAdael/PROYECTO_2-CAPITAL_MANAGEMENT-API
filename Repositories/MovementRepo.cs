using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    /// <summary>
    /// Repository for managing Movement entities.
    /// </summary>
    public class MovementRepo : IMovementRepo
    {
        private readonly MyAppDbContext _context;

        /// <summary>
        /// Constructor for MovementRepo
        /// </summary>
        /// <param name="context"></param>
        public MovementRepo(MyAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Method to get movements by organization ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Movement>> GetMovementsByIDOrg(int id)
        {
            return await _context.Movements.Where(u => u.Id == id).ToListAsync();
        }
        /// <summary>
        /// Method to get a movement by organization ID and movement number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nmov"></param>
        /// <returns></returns>
        public async Task<Movement> GetMovByNOrg(int id, int nmov)
        {
            var registro = await _context.Movements.FirstOrDefaultAsync(u => u.OrganizationId == id && u.NoMov == nmov);
            if (registro == null)
            {
                return null;
            }
            return registro;
        }
        /// <summary>
        /// Method to create a new movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        public async Task<bool> NewMovement(Movement movement)
        {
            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to update a movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        public async Task<bool> PatchMov(Movement movement)
        {
            _context.Movements.Update(movement);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to delete a movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMov(Movement movement)
        {
            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to get all movements for a specific organization
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>

        public async Task<IEnumerable<Movement>> GetAllMovements(int OrgId)
        {
            var myMovements = await _context.Movements
                .Where(o => o.OrganizationId == OrgId).OrderBy(o => o.NoMov)
                .ToListAsync();
            return myMovements;
        }
        /// <summary>
        /// Method to get all movements
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Movement>> GetAllMov()
        {
            return await _context.Movements.ToListAsync();
        }
    }
}
