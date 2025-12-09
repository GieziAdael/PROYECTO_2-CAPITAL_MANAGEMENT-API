using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

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
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeRepo"></param>
        /// <param name="mapper"></param>
        /// <param name="employeeService"></param>
        public EmployeeController(IEmployeeRepo employeeRepo, IMapper mapper,
            IEmployeeService employeeService)
        {
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _employeeService = employeeService;
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
            try
            {

                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var members = await _employeeService.AllMembersAOrganization(OrgId, tokenId);
                return Ok(members);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var register = await _employeeService.AddMember(OrgId, addMemberDto, tokenId);
                return CreatedAtAction(nameof(AddMemberToOrganization), new { id = register.Id }, register);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This endpoint updates the role of an existing member in a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="updateRoleDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("UpdateRoleMember/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRoleOfMember(int OrgId, [FromBody] FB_CreateEmployeeDto updateRoleDto)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _employeeService.UpdateRoleMember(OrgId, updateRoleDto, tokenId);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// This endpoint removes a member from a specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("RemoveMember/{OrgId:int}/{email}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveEmployee(int OrgId, string email)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _employeeService.RemoveMember(OrgId, email, tokenId);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
