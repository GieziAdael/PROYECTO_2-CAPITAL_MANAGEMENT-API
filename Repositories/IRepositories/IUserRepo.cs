using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;

namespace API_CAPITAL_MANAGEMENT.Repositories.IRepositories
{
    public interface IUserRepo
    {
        Task<bool> ExistsById(int id);
        Task<bool> ExistsByEmail(string email);
        Task<User> GetById(int id);
        Task<bool> NewUser(User user);
        Task<bool> ActUserPassword(User user);
        Task<bool> DeleteUser(User user);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<IEnumerable<User>> GetAllUsers();
    }
}
