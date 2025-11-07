using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CAPITAL_MANAGEMENT.Controllers
{
    /// <summary>
    /// This controller manages employee-related operations within organizations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for EmployeeController
        /// </summary>
        /// <param name="employeeRepo"></param>
        /// <param name="mapper"></param>
        public EmployeeController(IEmployeeRepo employeeRepo, IMapper mapper)
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// This endpoint retrieves all members of a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Members/{OrgId:int}", Name = "GetMembersByOrganizationId")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMembersOfOrganization(int OrgId)
        {
            //Validations
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if(! await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para ver los miembros de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if(role != "Owner" && role != "Admin")
                return BadRequest("No tienes permisos para ver los miembros de esta organización");
            //Actions
            // Assuming there's a method to get members by organization ID
            var members = await _employeeRepo.GetMembersByOrganizationId(OrgId);
            var membersDto = _mapper.Map<List<Employee>>(members);
            return Ok(membersDto);
        }

        /// <summary>
        /// This endpoint adds a new member to a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="addMemberDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("AddMember/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMemberToOrganization(int OrgId, [FromBody] FB_CreateEmployeeDto addMemberDto)
        {
            //Validations
            if(string.IsNullOrEmpty(addMemberDto.EmailUser) || string.IsNullOrEmpty(addMemberDto.Role))
                return BadRequest("Hay campos vacios");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if(! await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para agregar miembros a esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if(role != "Owner")
                return BadRequest("Solo el Owner puede agregar miembros a esta organización");
            if(await _employeeRepo.ExistsByEmailToOrg(OrgId, addMemberDto.EmailUser))
                return BadRequest("El usuario ya es miembro de esta organización");
            if(addMemberDto.Role != "Admin" && addMemberDto.Role != "Viewer")
                return BadRequest("El rol debe ser 'Admin' o 'Viewer'");
            //Actions
            var user = await _employeeRepo.GetByIdUserEmployee(addMemberDto.EmailUser);
            if(user == null)
                return BadRequest("El usuario con ese correo no existe");

            var newEmployee = new Employee
            {
                Role = addMemberDto.Role,
                UserId = user.Id,
                OrganizationId = OrgId
            };
            var register = _mapper.Map<Employee>(newEmployee);
            if(!await _employeeRepo.NewEmployeeToOrg(register))
                return BadRequest("Error al agregar el miembro a la organización");
            return CreatedAtAction(nameof(AddMemberToOrganization), new { id = register.Id }, register);
        }

        /// <summary>
        /// This endpoint updates the role of an existing member in a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="updateRoleDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateRoleMember/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoleOfMember(int OrgId, [FromBody] FB_CreateEmployeeDto updateRoleDto)
        {
            //Validations
            if(string.IsNullOrEmpty(updateRoleDto.EmailUser) || string.IsNullOrEmpty(updateRoleDto.Role))
                return BadRequest("Hay campos vacios");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if(! await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para actualizar roles en esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if(role != "Owner")
                return BadRequest("Solo el Owner puede actualizar roles en esta organización");
            if(updateRoleDto.Role != "Admin" && updateRoleDto.Role != "Viewer")
                return BadRequest("El rol debe ser 'Admin' o 'Viewer'");
            

            //Actions
            var user = await _employeeRepo.GetByIdUserEmployee(updateRoleDto.EmailUser);
            if(user == null)
                return BadRequest("El usuario con ese correo no existe");
            if(tokenId == user.Id)
                return BadRequest("El Owner no puede cambiar su propio rol");

            var employee = await _employeeRepo.BodyByEmailToOrg(OrgId, updateRoleDto.EmailUser);
            var register = _mapper.Map<Employee>(employee);
            register.Id = employee.Id;
            register.Role = updateRoleDto.Role;

            if (!await _employeeRepo.PatchRoleEmployeeToOrg(register))
                return BadRequest("Error al actualizar el rol del miembro");
            return Ok(register);
        }


        /// <summary>
        /// This endpoint removes a member from a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("RemoveMember/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveEmployee(int OrgId, string email)
        {
            //Validations
            if(string.IsNullOrEmpty(email))
                return BadRequest("El correo no puede estar vacío");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if(! await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para eliminar miembros de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if(role != "Owner")
                return BadRequest("Solo el Owner puede eliminar miembros de esta organización");

            //Actions
            var user = await _employeeRepo.BodyByEmailToOrg(OrgId, email);
            if(user == null)
                return BadRequest("El usuario no es miembro de esta organización");
            if(user.UserId == tokenId)
                return BadRequest("El Owner no puede eliminarse a sí mismo");

            var register = _mapper.Map<Employee>(user);
            if (!await _employeeRepo.DeleteEmployeeToOrg(register))
                return BadRequest("Error al eliminar el miembro de la organización");
            return Ok("Miembro eliminado correctamente");
        }
    }
}
