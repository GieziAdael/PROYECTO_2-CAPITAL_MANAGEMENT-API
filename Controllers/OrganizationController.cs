using API_CAPITAL_MANAGEMENT.Data;
using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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
        private readonly IOrganizationService _organizationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationRepo"></param>
        /// <param name="employeeRepo"></param>
        /// <param name="movementRepo"></param>
        /// <param name="mapper"></param>
        /// <param name="organizationService"></param>
        public OrganizationController
            (IOrganizationRepo organizationRepo,IEmployeeRepo employeeRepo, IMovementRepo movementRepo ,IMapper mapper,
            IOrganizationService organizationService)
        {
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
            _organizationService = organizationService;
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
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var organization = await _organizationService.MyOrganizations(tokenId);
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Find organizations I am affiliated with
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Others")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOtherOrganizations()
        {
            try
            {

                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var organizations = await _organizationService.OrganizationsAfilied(tokenId);
                return Ok(organizations);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

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
        public async Task<IActionResult> RegisterOrganization([FromBody] FB_CreateOrgDto createOrgDto)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var organization = await _organizationService.CreateOrganization(createOrgDto, tokenId);
                return Created("GetOrganizationById", new
                {
                    Id = organization.Id,
                    NameOrg = organization.NameOrg,
                    OwnerId = organization.UserId,
                    Message = "Se ha creado la organizacion ;)"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var organization = await _organizationService.LoginOrganization(fB_LoginOrgDto, tokenId);
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrgPassword(int orgId, string newPassword)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _organizationService.UpdatePasswordOrganization(orgId, newPassword, tokenId);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _organizationService.DeleteOrganization(orgId, tokenId);
                return NoContent();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
