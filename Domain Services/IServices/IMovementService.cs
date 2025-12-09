using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Domain_Services.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMovementService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<IEnumerable<Movement>> MyMovements(int OrgId, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="createMovDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Movement> CreateMovement(int OrgId, FB_CreateMovDto createMovDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<decimal> CalculateBalance(int OrgId, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="NoMov"></param>
        /// <param name="fB_UpdateMovDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<Movement> UpdateMovement(int OrgId,int NoMov ,FB_CreateMovDto fB_UpdateMovDto, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="NoMov"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task DeleteMovement(int OrgId,int NoMov,int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task DeleteAllMovements(int OrgId,int tokenId);
    }
}
