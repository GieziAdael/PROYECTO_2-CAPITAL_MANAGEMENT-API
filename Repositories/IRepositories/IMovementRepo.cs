using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    public interface IMovementRepo
    {
        Task<IEnumerable<Movement>> GetMovementsByIDOrg(int id);
        Task<Movement> GetMovByNOrg(int id, int nmov);
        Task<bool> NewMovement(Movement movement);
        Task<bool> PatchMov(Movement movement);
        Task<bool> DeleteMov(Movement movement);

        Task<IEnumerable<Movement>> GetAllMovements(int OrgId);
        Task<IEnumerable<Movement>> GetAllMov();

    }
}
