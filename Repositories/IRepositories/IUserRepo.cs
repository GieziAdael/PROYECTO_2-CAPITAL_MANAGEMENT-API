using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    /// <summary>
    /// Interface for User Repository
    /// </summary>
    public interface IUserRepo
    {
        /// <summary>
        /// Method to check if a user exists by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ExistsById(int id);
        /// <summary>
        /// Method to check if a user exists by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> ExistsByEmail(string email);
        /// <summary>
        /// Method to get a user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User> GetById(int id);
        /// <summary>
        /// Method to create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> NewUser(User user);
        /// <summary>
        /// Method to update a user's password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> ActUserPassword(User user);
        /// <summary>
        /// Method to delete a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<bool> DeleteUser(User user);
        /// <summary>
        /// Method for user login
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns></returns>
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        /// <summary>
        /// Method to get all users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<User>> GetAllUsers();
    }
}
