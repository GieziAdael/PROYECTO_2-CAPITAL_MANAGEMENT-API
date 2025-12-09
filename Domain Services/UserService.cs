using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API_CAPITAL_MANAGEMENT.Domain_Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRepo"></param>
        /// <param name="organizationRepo"></param>
        /// <param name="employeeRepo"></param>
        /// <param name="movementRepo"></param>
        /// <param name="mapper"></param>
        public UserService(IUserRepo userRepo, IOrganizationRepo organizationRepo, IEmployeeRepo employeeRepo, IMovementRepo movementRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRegisterDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> RegisterUserAsync(FB_CreateUserDto userRegisterDto)
        {
            //Rules and validations
            //Email or Password cannot be empty
            if (string.IsNullOrEmpty(userRegisterDto.Email) || string.IsNullOrEmpty(userRegisterDto.Password))
                throw new Exception("El Email y Password son requeridos");
            //Email valid format
            if (!new EmailAddressAttribute().IsValid(userRegisterDto.Email))
                throw new Exception("El formato del Email es inválido");
            //Exists email
            if (await _userRepo.ExistsByEmail(userRegisterDto.Email.ToLower().Trim().Normalize()))
                throw new Exception("El Email ya se encuentra en uso");
            //Password must be longer than 7 characters
            if (userRegisterDto.Password.Length < 8)
                throw new Exception("La contraseña debe tener al menos 8 caracteres");
            //The email must be less than 350 characters
            if (userRegisterDto.Email.Length > 350)
                throw new Exception("El Email no debe exceder los 350 caracteres");

            //Actions
            var newUser = new User
            {
                EmailNormalized = userRegisterDto.Email,
                PasswordHash = userRegisterDto.Password
            };
            var register = _mapper.Map<User>(newUser);
            var registerModel = await _userRepo.NewUser(register);

            if (!await _userRepo.ExistsByEmail(userRegisterDto.Email.ToLower().Trim().Normalize()))
            {
                throw new Exception("Error al registrar el usuario");
            }

            return register;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<UserLoginResponseDto> LoginUserAsync(UserLoginDto userLoginDto)
        {
            //Rules and validations
            //Email or Password cannot be empty
            if (string.IsNullOrEmpty(userLoginDto.Email) || string.IsNullOrEmpty(userLoginDto.Password))
                throw new Exception("El Email y Password son requeridos");

            //Actions
            var user = await _userRepo.Login(userLoginDto);
            if (user.User == null)
            {
                throw new Exception(user.Message);
            }

            return user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> ModifyPasswordUserAsync(string newPassword, int tokenId)
        {
            //Verifications
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("La nueva contraseña es requerida");
            if (newPassword.Length < 8)
                throw new Exception("La nueva contraseña debe tener al menos 8 caracteres");
            var user = await _userRepo.GetById(tokenId);
            if (user == null)
                throw new Exception("Usuario no encontrado");
            //Actions
            user.PasswordHash = newPassword;
            if (!await _userRepo.ActUserPassword(user))
                throw new Exception("Error al modificar la contraseña");

            return user;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteMyAccountAsync(int tokenId)
        {
            var user = await _userRepo.GetById(tokenId);
            if (user == null)
                throw new Exception("Usuario no encontrado");
            //Delete all movements, employees and organizations related to the user
            var organizations = await _organizationRepo.GetAllMyOrginations(tokenId);
            foreach (var orgs in organizations)
            {
                var employees = await _employeeRepo.GetMembersByOrganizationId(orgs.Id);
                foreach (var emp in employees)
                {
                    await _employeeRepo.DeleteEmployeeToOrg(emp);
                }
                var movements = await _movementRepo.GetAllMovements(orgs.Id);
                foreach (var mov in movements)
                {
                    await _movementRepo.DeleteMov(mov);
                }
                await _organizationRepo.DeleteOrg(orgs);
            }
            //Remove me from the employees
            var meEmployee = await _employeeRepo.SearchMeById(tokenId);
            if (meEmployee is not null)
                foreach (var emp in meEmployee)
                {
                    await _employeeRepo.DeleteEmployeeToOrg(emp);
                }

            //Finally delete the user
            if (!await _userRepo.DeleteUser(user))
            {
                throw new Exception("Error al eliminar el usuario");
            }

        }
    }
}
