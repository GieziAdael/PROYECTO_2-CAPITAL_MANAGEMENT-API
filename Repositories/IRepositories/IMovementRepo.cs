using API_CAPITAL_MANAGEMENT.Entities;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    /// <summary>
    /// Interface for Movement Repository
    /// </summary>
    public interface IMovementRepo
    {
        /// <summary>
        /// Method to get movements by organization ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Movement>> GetMovementsByIDOrg(int id);
        /// <summary>
        /// Method to get a movement by organization ID and movement number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nmov"></param>
        /// <returns></returns>
        Task<Movement> GetMovByNOrg(int id, int nmov);
        /// <summary>
        /// Method to add a new movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        Task<bool> NewMovement(Movement movement);
        /// <summary>
        /// Method to update a movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        Task<bool> PatchMov(Movement movement);
        /// <summary>
        /// Method to delete a movement
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        Task<bool> DeleteMov(Movement movement);

        /// <summary>
        /// Method to get all movements for an organization
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        Task<IEnumerable<Movement>> GetAllMovements(int OrgId);
        /// <summary>
        /// Method to get all movements
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Movement>> GetAllMov();

    }
}
