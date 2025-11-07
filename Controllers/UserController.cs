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
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

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

        //Login user
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

            //Finally delete the user
            await _userRepo.DeleteUser(user);
            return Ok("Cuenta eliminada exitosamente");
        }
    }
}
