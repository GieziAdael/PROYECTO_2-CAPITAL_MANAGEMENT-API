using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_CAPITAL_MANAGEMENT.Repositories
{
    /// <summary>
    /// Repository for managing User entities
    /// </summary>
    public class UserRepo : IUserRepo
    {
        private readonly MyAppDbContext _context;
        private string? secretKey;

        /// <summary>
        /// Constructor for UserRepo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        public UserRepo(MyAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        }
        /// <summary>
        /// Method to check if a user exists by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ExistsById(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
        /// <summary>
        /// Method to check if a user exists by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> ExistsByEmail(string email)
        {
            var emailN = email.ToLower().Trim();
            return await _context.Users.AnyAsync(u => u.EmailNormalized.Trim().ToLower() == emailN);
        }
        /// <summary>
        /// Method to get a user by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetById(int id)
        {
            var registro = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(registro == null)
            {
                return new User { Id = 0, EmailNormalized = "", PasswordHash = "" };
            }
            return registro;
        }
        /// <summary>
        /// Method to create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> NewUser(User user)
        {
            user.EmailNormalized = user.EmailNormalized.Trim().ToLower().Normalize();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to update a user's password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> ActUserPassword(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method to delete a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Method for user login
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            if (string.IsNullOrEmpty(userLoginDto.Email))
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null,
                    Message = "El Email es requerido"
                };
            }
            var user = await _context.Users.FirstOrDefaultAsync<User>(u => u.EmailNormalized.ToLower().Trim() == userLoginDto.Email.ToLower().Trim().Normalize());
            if (user == null)
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null,
                    Message = "El Username no fue encontrado"
                };
            }
            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash))
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null,
                    Message = "Las credenciales son incorrectas"
                };
            }
            //Desde aqui generamos el Token JWT
            var handlerToken = new JwtSecurityTokenHandler();
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("Secret key, no esta configurada");
            }
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.EmailNormalized),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handlerToken.CreateToken(tokenDescriptor);
            return new UserLoginResponseDto()
            {
                User = new UserRegisterDto()
                {
                    EmailNormalized = user.EmailNormalized,
                    PasswordHash = user.PasswordHash
                },
                Token = handlerToken.WriteToken(token),
                
                Message = "Usuario logeado correctamente"
            };
        }
        /// <summary>
        /// Method to get all users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
