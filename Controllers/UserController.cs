using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace API_CAPITAL_MANAGEMENT.Controllers
{
    /// <summary>
    /// Provides endpoints for managing user accounts, including registration, login, password modification,  and
    /// account deletion. This controller handles user-related operations and enforces validation rules  for user input.
    /// </summary>
    /// <remarks>The <see cref="UserController"/> class is an API controller that exposes endpoints for user
    /// management.  It includes functionality for registering new users, authenticating existing users, modifying user 
    /// passwords, and deleting user accounts along with their associated data.   This controller relies on
    /// dependency-injected repositories for data access and a mapper for object  transformations. It also enforces
    /// validation rules for user input, such as ensuring email format  validity and password length requirements. 
    /// Authorization is required for certain endpoints, such as modifying passwords and deleting accounts.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class with the specified repositories and
        /// mapper.
        /// </summary>
        /// <param name="userRepo">The repository used to manage user data.</param>
        /// <param name="organizationRepo">The repository used to manage organization data.</param>
        /// <param name="employeeRepo">The repository used to manage employee data.</param>
        /// <param name="movementRepo">The repository used to manage movement data.</param>
        /// <param name="mapper">The mapper used for object-to-object mapping.</param>
        public UserController
            (IUserRepo userRepo, IOrganizationRepo organizationRepo, IEmployeeRepo employeeRepo, IMovementRepo movementRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
        }

        //[Authorize]
        //[HttpGet("all")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var register = await _userRepo.GetAllUsers();
        //    var registerDto = _mapper.Map<List<UserDto>>(register);
        //    return Ok(registerDto);
        //}

        //Register a new user

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <remarks>This method validates the provided email and password before attempting to register
        /// the user. The following conditions must be met: <list type="bullet"> <item><description>The email must not
        /// be empty and must be in a valid format.</description></item> <item><description>The password must not be
        /// empty and must be at least 8 characters long.</description></item> <item><description>The email must not
        /// exceed 350 characters.</description></item> <item><description>The email must not already be in
        /// use.</description></item> </list> If the registration is successful, the method returns the created user's
        /// ID and normalized email.</remarks>
        /// <param name="userRegisterDto">An object containing the user's registration details, including email and password.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the operation. Returns <see
        /// cref="StatusCodes.Status201Created"/> if the user is successfully registered, <see
        /// cref="StatusCodes.Status400BadRequest"/> if the input is invalid or the registration fails.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] FB_CreateUserDto userRegisterDto)
        {
            //Rules and validations
            //Email or Password cannot be empty
            if (string.IsNullOrEmpty(userRegisterDto.Email) || string.IsNullOrEmpty(userRegisterDto.Password))
                return BadRequest("El Email y Password son requeridos");
            //Email valid format
            if (!new EmailAddressAttribute().IsValid(userRegisterDto.Email))
                return BadRequest("El formato del Email es inválido");
            //Exists email
            if (await _userRepo.ExistsByEmail(userRegisterDto.Email.ToLower().Trim().Normalize()))
                return BadRequest("El Email ya se encuentra en uso");
            //Password must be longer than 7 characters
            if (userRegisterDto.Password.Length < 8)
                return BadRequest("La contraseña debe tener al menos 8 caracteres");
            //The email must be less than 350 characters
            if (userRegisterDto.Email.Length > 350)
                return BadRequest("El Email no debe exceder los 350 caracteres");

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
                return BadRequest("Error al registrar el usuario");
            }
            return Created("GetById", new
            {
                Id = register.Id,
                Email = register.EmailNormalized,
                Message = "Usuario registrado exitosamente"
            });
        }

        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// </summary>
        /// <remarks>This method validates the provided email and password before attempting to
        /// authenticate the user. Ensure that both fields are supplied in the <paramref name="userLoginDto"/>
        /// object.</remarks>
        /// <param name="userLoginDto">An object containing the user's email and password.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the login operation: <list type="bullet">
        /// <item><description><see cref="StatusCodes.Status200OK"/> if the login is successful, with the authenticated
        /// user details.</description></item> <item><description><see cref="StatusCodes.Status401Unauthorized"/> if the
        /// credentials are invalid.</description></item> <item><description><see
        /// cref="StatusCodes.Status403Forbidden"/> if access is denied for other reasons.</description></item>
        /// <item><description><see cref="StatusCodes.Status400BadRequest"/> if the email or password is
        /// missing.</description></item> </list></returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            //Rules and validations
            //Email or Password cannot be empty
            if (string.IsNullOrEmpty(userLoginDto.Email) || string.IsNullOrEmpty(userLoginDto.Password))
                return BadRequest("El Email y Password son requeridos");

            //Actions
            var user = await _userRepo.Login(userLoginDto);
            if (user.User == null)
            {
                return Unauthorized(user.Message);
            }

            return Ok(user);
        }

        /// <summary>
        /// Updates the password of the currently authenticated user.
        /// </summary>
        /// <remarks>This method requires the user to be authenticated. The new password must meet the
        /// minimum length  requirement of 8 characters. If the user is not found or the password update fails, a bad
        /// request  response is returned.</remarks>
        /// <param name="newPassword">The new password to set for the user. Must be at least 8 characters long.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="StatusCodes.Status200OK"/> if the password is successfully updated,  or <see
        /// cref="StatusCodes.Status400BadRequest"/> if the input is invalid or the operation fails.</returns>
        [Authorize]
        [HttpPut("ModifyMyPassword/{newPassword}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyMyPassword(string newPassword)
        {
            //Verifications
            if (string.IsNullOrEmpty(newPassword))
                return BadRequest("La nueva contraseña es requerida");
            if (newPassword.Length < 8)
                return BadRequest("La nueva contraseña debe tener al menos 8 caracteres");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var user = await _userRepo.GetById(tokenId);
            if (user == null)
                return BadRequest("Usuario no encontrado");
            //Actions
            user.PasswordHash = newPassword;
            if (!await _userRepo.ActUserPassword(user))
                return BadRequest("Error al modificar la contraseña");
            return Ok(user);
        }

        /// <summary>
        /// Deletes the currently authenticated user's account and all associated data.
        /// </summary>
        /// <remarks>This operation removes the user's account along with all related data, including
        /// organizations, employees, and movements associated with the user. Once the account is deleted, the action is
        /// irreversible.</remarks>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see
        /// cref="StatusCodes.Status200OK"/> if the account was successfully deleted, or <see
        /// cref="StatusCodes.Status400BadRequest"/> if the user could not be found.</returns>
        [Authorize]
        [HttpDelete("DeleteMyAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMyAccount()
        {
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var user = await _userRepo.GetById(tokenId);
            if (user == null)
                return BadRequest("Usuario no encontrado");
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
            if(meEmployee is not null)
                foreach (var emp in meEmployee)
                {
                    await _employeeRepo.DeleteEmployeeToOrg(emp);
                }

            //Finally delete the user
            if (!await _userRepo.DeleteUser(user))
            {
                return BadRequest("Error al eliminar el usuario");
            }
            return Ok("Cuenta eliminada exitosamente");
        }
    }
}
