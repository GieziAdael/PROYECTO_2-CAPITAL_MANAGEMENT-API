using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Controllers
{
    /// <summary>
    /// Provides endpoints for managing organizations, including creating, updating, deleting, and retrieving
    /// organizations, as well as handling organization login and membership.
    /// </summary>
    /// <remarks>This controller is designed to handle organization-related operations for authenticated
    /// users.  It includes functionality for creating new organizations, retrieving a user's organizations, updating
    /// organization details, and managing organization memberships.  The controller enforces authorization for all
    /// endpoints, ensuring that only authenticated users can access the operations.  Additionally, certain actions,
    /// such as updating or deleting an organization, require the user to have specific roles (e.g., "Owner") within the
    /// organization.  Common validation rules are applied to ensure data integrity, such as checking for non-empty
    /// names and passwords, enforcing password length requirements, and verifying the existence of organizations before
    /// performing operations.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationController"/> class.
        /// </summary>
        /// <param name="organizationRepo">The repository used to manage organization-related data.</param>
        /// <param name="employeeRepo">The repository used to manage employee-related data.</param>
        /// <param name="movementRepo">The repository used to manage movement-related data.</param>
        /// <param name="mapper">The object mapper used to map between domain models and DTOs.</param>
        public OrganizationController
            (IOrganizationRepo organizationRepo,IEmployeeRepo employeeRepo, IMovementRepo movementRepo ,IMapper mapper)
        {
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a list of organizations associated with the currently authenticated user.
        /// </summary>
        /// <remarks>This method requires the user to be authenticated and authorized. The organizations
        /// returned are specific to the user identified by the token in the request. The response is cached for 10
        /// seconds to improve performance.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a list of organizations associated with the user in the form of a
        /// collection of <see cref="Organization"/> objects, with a status code of <see
        /// cref="StatusCodes.Status200OK"/>. If the request is invalid, a status code of <see
        /// cref="StatusCodes.Status400BadRequest"/> is returned.</returns>
        [Authorize]
        [HttpGet("MyOrganizations", Name = "GetAllMyOrginations")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllOrganizations()
        {
            int TokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var organizations = await _organizationRepo.GetAllMyOrginations(TokenId);
            var organizationsDto = _mapper.Map<List<FB_ListOrganizationsDto>>(organizations);
            return Ok(organizationsDto);
        }

        /// <summary>
        /// Registers a new organization and assigns the current user as the owner of the organization.
        /// </summary>
        /// <remarks>The following validation rules apply: <list type="bullet"> <item><description>The
        /// organization's name and password cannot be null or empty.</description></item> <item><description>The
        /// organization's password must be at least 8 characters long.</description></item> <item><description>The
        /// organization's name must not exceed 150 characters.</description></item> <item><description>The
        /// organization's name must be unique.</description></item> </list> This method requires the user to be
        /// authenticated and authorized. The authenticated user's ID is used to associate the user as the owner of the
        /// newly created organization.</remarks>
        /// <param name="createOrgDto">An object containing the details of the organization to be created, including the organization's name and
        /// password.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> representing the result of the operation.  Returns: <list type="bullet">
        /// <item><description><see cref="StatusCodes.Status201Created"/> if the organization is successfully
        /// created.</description></item> <item><description><see cref="StatusCodes.Status400BadRequest"/> if the input
        /// data is invalid or the organization name is already in use.</description></item> </list></returns>
        [Authorize]
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterUser([FromBody] FB_CreateOrgDto createOrgDto)
        {
            //Rules and validations
            //Name or Password cannot be empty
            if (string.IsNullOrEmpty(createOrgDto.NameOrganization) || string.IsNullOrEmpty(createOrgDto.PasswordOrganization))
                return BadRequest("El Nombre y Password son requeridos");
            //Exists Name
            if (await _organizationRepo.ExistsOrgByName(createOrgDto.NameOrganization))
                return BadRequest($"El Nombre '{createOrgDto.NameOrganization}' ya se encuentra en uso");
            //Password must be longer than 7 characters
            if (createOrgDto.PasswordOrganization.Length < 8)
                return BadRequest("La contraseña debe tener al menos 8 caracteres");
            //The Name must be less than 150 characters
            if (createOrgDto.NameOrganization.Length > 150)
                return BadRequest("El Nombre no debe exceder los 150 caracteres");

            //Actions
            //Create Organization

            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var newOrganization = new Organization
            {
                NameOrg = createOrgDto.NameOrganization,
                PasswordOrgHash = createOrgDto.PasswordOrganization,
                UserId = tokenId
            };
            var register = _mapper.Map<Organization>(newOrganization);
            var registerModel = await _organizationRepo.NewOrganitation(register);

            if (!await _organizationRepo.ExistsOrgByName(createOrgDto.NameOrganization))
            {
                return BadRequest("Error al registrar la Organizacion");
            }

            //Create Owner Organization Role
            var newOwnerOrg = new Employee
            {
                Role = "Owner",
                UserId = tokenId,
                OrganizationId = register.Id
            };
            var registerE = _mapper.Map<Employee>(newOwnerOrg);
            var registerModelE = await _employeeRepo.NewEmployeeToOrg(registerE);

            return Created("GetById", new
            {
                IdOrg = register.Id,
                NameOrg = register.NameOrg,
                Message = "Organizacion registrada exitosamente",
                IdEmployee = registerE.Id,
                Role = registerE.UserId,
                OrgId = registerE.OrganizationId,
                Message2 = $"Te asignaste como {newOwnerOrg.Role} en tu organizacion llamada {register.NameOrg}"

            });
        }

        /// <summary>
        /// Updates the password for the specified organization.
        /// </summary>
        /// <remarks>This method requires the user to be authorized and to have the "Owner" role for the
        /// specified organization.  The user must also belong to the organization being updated. If the organization
        /// does not exist, or if the  user does not have the required permissions, the method will return a bad request
        /// response.</remarks>
        /// <param name="orgId">The unique identifier of the organization whose password is being updated. Must be greater than 0.</param>
        /// <param name="newPassword">The new password to set for the organization. Must be at least 8 characters long.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="StatusCodes.Status200OK"/> if the password is successfully updated, or  <see
        /// cref="StatusCodes.Status400BadRequest"/> if the input is invalid or the operation fails.</returns>
        [Authorize]
        [HttpPut("UpdatePassword/{orgId:int}/{newPassword}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrgPassword(int orgId, string newPassword)
        {
            //Rules and validations
            if(orgId <= 0)
                return BadRequest("El Id de la organización es inválido");
            if (string.IsNullOrEmpty(newPassword))
                return BadRequest("La nueva contraseña es requerida");
            if (newPassword.Length < 8)
                return BadRequest("La nueva contraseña debe tener al menos 8 caracteres");
            if (!await _organizationRepo.ExistsOrgById(orgId))
                return BadRequest("La organización no existe");

            //Is the user owner of the organization?
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var employee = await _employeeRepo.GetByIdUserEmployee(tokenId);
            if(employee == null)
                return BadRequest("No se encontro dicha organizacion");
            if (employee.Role != "Owner" && employee.OrganizationId != orgId)
                return BadRequest("No tienes permisos para actualizar la contraseña de esta organización");

            //Actions
            var registro = await _organizationRepo.GetOrganizationById(orgId);
            if(registro == null)
                return BadRequest("La organización no existe");

            var organization = _mapper.Map<Organization>(registro);
            organization.Id = orgId;
            organization.PasswordOrgHash = newPassword;

            if (! await _organizationRepo.PatchOrg(organization))
                return BadRequest("Error al actualizar la contraseña de la organización");

            return Ok(organization);
        }

        /// <summary>
        /// Authenticates an organization based on the provided credentials.
        /// </summary>
        /// <remarks>This method requires the caller to be authorized. Ensure that the provided
        /// credentials are valid  and that the user is a member of the organization before calling this
        /// method.</remarks>
        /// <param name="fB_LoginOrgDto">An object containing the organization's login credentials, including the organization's name and password.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.  Returns <see
        /// cref="StatusCodes.Status200OK"/> if the authentication is successful,  or <see
        /// cref="StatusCodes.Status400BadRequest"/> if the credentials are invalid,  the user is not a member of the
        /// organization, or required fields are missing.</returns>
        [Authorize]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginOrg([FromBody] FB_CreateOrgDto fB_LoginOrgDto)
        {
            //Rules and validations
            if (string.IsNullOrEmpty(fB_LoginOrgDto.NameOrganization) || string.IsNullOrEmpty(fB_LoginOrgDto.PasswordOrganization))
                return BadRequest("El Nombre y Password son requeridos");

            var organization = await _organizationRepo.GetOrganization(fB_LoginOrgDto.NameOrganization, fB_LoginOrgDto.PasswordOrganization);
            if (organization == null)
                return BadRequest("Nombre o contraseña incorrectos");
            //Actions

            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var employee = await _employeeRepo.IsMember(tokenId, organization.Id);

            if (!employee)
                return BadRequest("No eres miembro de esta organización");

            return Ok(organization);
        }

        /// <summary>
        /// Deletes an organization and all its associated data, including employees and movements.
        /// </summary>
        /// <remarks>This operation requires the user to be authorized and to have the "Owner" role within
        /// the organization being deleted.  All employees and movements associated with the organization will also be
        /// deleted as part of this operation.</remarks>
        /// <param name="orgId">The unique identifier of the organization to delete. Must be a positive integer.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet">
        /// <item><description><see cref="StatusCodes.Status200OK"/> if the organization is successfully
        /// deleted.</description></item> <item><description><see cref="StatusCodes.Status400BadRequest"/> if the
        /// organization does not exist, the user lacks permissions, or the input is invalid.</description></item>
        /// </list></returns>
        [Authorize]
        [HttpDelete("Delete/{orgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteOrganization(int orgId)
        {
            //Rules and validations
            if (orgId <= 0)
                return BadRequest("El Id de la organización es inválido");
            if (!await _organizationRepo.ExistsOrgById(orgId))
                return BadRequest("La organización no existe");
            //Is the user owner of the organization?
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var employee = await _employeeRepo.GetByIdUserEmployee(tokenId);
            if (employee == null)
                return BadRequest("No se encontro dicha organizacion");
            if (employee.Role != "Owner" && employee.OrganizationId != orgId)
                return BadRequest("No tienes permisos para eliminar esta organización");
            //Actions
            var registro = await _organizationRepo.GetOrganizationById(orgId);
            if (registro == null)
                return BadRequest("La organización no existe");
            //Delete Employees associated with the organization
            var employees = await _employeeRepo.GetMembersByOrganizationId(orgId);
            foreach (var emp in employees)
            {
                await _employeeRepo.DeleteEmployeeToOrg(emp);
            }
            //Delete Movements associated with the organization
            var movements = await _movementRepo.GetAllMovements(orgId);
            foreach (var mov in movements)
            {
                await _movementRepo.DeleteMov(mov);
            }

            //Delete Organization
            var organization = _mapper.Map<Organization>(registro);
            organization.Id = orgId;
            if (!await _organizationRepo.DeleteOrg(organization))
                return BadRequest("Error al eliminar la organización");
            return Ok($"Organización con Id {orgId} eliminada exitosamente");
        }

        ////Token
        //[Authorize]
        //[HttpPost("Token")]
        //public IActionResult TokenOrg()
        //{
        //    var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    return Ok(claims);
        //}
    }
}
