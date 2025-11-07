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
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

        public OrganizationController
            (IOrganizationRepo organizationRepo,IEmployeeRepo employeeRepo, IMovementRepo movementRepo ,IMapper mapper)
        {
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("MyOrganizations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllOrganizations()
        {
            int TokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");

            var organizations = await _organizationRepo.GetAllMyOrginations(TokenId);
            var organizationsDto = _mapper.Map<List<Organization>>(organizations);
            return Ok(organizationsDto);
        }

        //Create a new organization for  user
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

        //Update password organization
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

        //Login organization
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
