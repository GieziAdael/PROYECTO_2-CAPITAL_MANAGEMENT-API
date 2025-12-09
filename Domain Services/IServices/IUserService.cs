using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Domain_Services.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRegisterDto"></param>
        /// <returns></returns>
        Task<User> RegisterUserAsync(FB_CreateUserDto userRegisterDto);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns></returns>
        Task<UserLoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task<User> ModifyPasswordUserAsync(string newPassword, int tokenId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        Task DeleteMyAccountAsync(int tokenId);
    }
}
