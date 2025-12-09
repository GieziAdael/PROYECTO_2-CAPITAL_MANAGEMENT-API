using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
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
    /// Provides endpoints for managing financial movements within an organization.
    /// </summary>
    /// <remarks>The <see cref="MovementController"/> class exposes a set of API endpoints for performing CRUD
    /// operations  on financial movements, as well as calculating balances. These endpoints are secured with
    /// authorization  and are designed to ensure that only users with appropriate roles can access or modify
    /// data.</remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class MovementController : ControllerBase
    {
        private readonly IMovementRepo _movementRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMapper _mapper;
        private readonly IMovementService _movementService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movementRepo"></param>
        /// <param name="employeeRepo"></param>
        /// <param name="mapper"></param>
        /// <param name="movementService"></param>
        public MovementController(IMovementRepo movementRepo, IEmployeeRepo employeeRepo, IMapper mapper,
            IMovementService movementService)
        {
            _movementRepo = movementRepo;
            _employeeRepo = employeeRepo;
            _mapper = mapper;
            _movementService = movementService;
        }

        /// <summary>
        /// Retrieves all movements associated with the specified organization.
        /// </summary>
        /// <remarks>This method requires the caller to be authorized and a member of the specified
        /// organization.  The caller must have one of the following roles within the organization: "Owner", "Admin", or
        /// "Viewer". The response is cached for 20 seconds to improve performance.</remarks>
        /// <param name="OrgId">The unique identifier of the organization whose movements are to be retrieved.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of movements if the request is successful,  or a <see
        /// cref="BadRequestResult"/> if the user does not have the necessary permissions or access.</returns>
        [Authorize]
        [HttpGet("MyMovements/{OrgId:int}", Name = "GetAllMovements")]
        [ResponseCache(Duration = 20)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllMovements(int OrgId)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var movements = await _movementService.MyMovements(OrgId, tokenId);
                return Ok(movements);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new financial movement for the specified organization.
        /// </summary>
        /// <remarks>The user must have the "Owner" or "Admin" role within the specified organization to
        /// create a movement.  The <paramref name="createMovDto"/> must include a non-empty title and type, and the
        /// amount must be non-negative.  The type of movement must be either "Ingreso" or "Egreso".</remarks>
        /// <param name="OrgId">The unique identifier of the organization for which the movement is being created.</param>
        /// <param name="createMovDto">An object containing the details of the movement to be created, including the title, type, amount, and
        /// description.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="StatusCodes.Status201Created"/> if the movement is successfully created,  <see
        /// cref="StatusCodes.Status400BadRequest"/> if the input is invalid or the user lacks the necessary
        /// permissions,  and <see cref="StatusCodes.Status200OK"/> for other successful outcomes.</returns>
        [Authorize]
        [HttpPost("Create/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMovement(int OrgId, [FromBody] FB_CreateMovDto createMovDto)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var movement = await _movementService.CreateMovement(OrgId, createMovDto, tokenId);

                return CreatedAtAction(nameof(CreateMovement), new { id = movement.Id }, movement);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the financial balance for the specified organization.
        /// </summary>
        /// <remarks>The balance is calculated by summing all movements of type "Ingreso" (income) and
        /// subtracting all movements of type "Egreso" (expense) associated with the specified organization. The result
        /// is cached for 10 seconds to improve performance.</remarks>
        /// <param name="OrgId">The unique identifier of the organization whose balance is to be calculated.</param>
        /// <returns>An <see cref="IActionResult"/> containing the calculated balance in a JSON object with the key "Balance" if
        /// the operation is successful, or a <see cref="BadRequestResult"/> if the user does not have the necessary
        /// permissions or access to the organization.</returns>
        [Authorize]
        [HttpGet("CalculateBalance/{OrgId:int}", Name = "CalculateBalance")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalculateBalance(int OrgId)
        {
            try
            {

                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var balance = await _movementService.CalculateBalance(OrgId, tokenId);
                return Ok(new { Balance = balance });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing movement record for the specified organization.
        /// </summary>
        /// <remarks>This method requires the user to be authorized and have the "Owner" or "Admin" role
        /// within the specified organization.</remarks>
        /// <param name="OrgId">The unique identifier of the organization to which the movement belongs.</param>
        /// <param name="NoMov">The unique number of the movement to be updated.</param>
        /// <param name="fB_UpdateMovDto">An object containing the updated details of the movement. The <see cref="FB_CreateMovDto.TitleMov"/> and
        /// <see cref="FB_CreateMovDto.TypeMov"/> properties must not be null or empty. The <see
        /// cref="FB_CreateMovDto.AmountMov"/> must be non-negative, and the <see cref="FB_CreateMovDto.TypeMov"/> must
        /// be either "Ingreso" or "Egreso".</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet">
        /// <item><description><see cref="StatusCodes.Status200OK"/> if the movement was successfully
        /// updated.</description></item> <item><description><see cref="StatusCodes.Status400BadRequest"/> if the input
        /// is invalid, the user lacks permissions, or the movement does not exist.</description></item> </list></returns>
        [Authorize]
        [HttpPut("Update/{OrgId:int}/{NoMov:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMovement
            (int OrgId, int NoMov, [FromBody] FB_CreateMovDto fB_UpdateMovDto)
        {
            try
            {

                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                var movement = await _movementService.UpdateMovement(OrgId, NoMov, fB_UpdateMovDto, tokenId);
                return Ok(movement);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a movement associated with the specified organization and movement number.
        /// </summary>
        /// <remarks>This method requires the user to be authorized and have either the "Owner" or "Admin"
        /// role  within the specified organization. If the movement does not exist or the user lacks the  necessary
        /// permissions, the method returns a bad request response.</remarks>
        /// <param name="OrgId">The unique identifier of the organization to which the movement belongs.</param>
        /// <param name="NoMov">The movement number within the organization to be deleted.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.  Returns <see
        /// cref="StatusCodes.Status200OK"/> if the movement is successfully deleted, or  <see
        /// cref="StatusCodes.Status400BadRequest"/> if the operation fails due to invalid input,  insufficient
        /// permissions, or other errors.</returns>
        [Authorize]
        [HttpDelete("Delete/{OrgId:int}/{NoMov:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMovement(int OrgId, int NoMov)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _movementService.DeleteMovement(OrgId,NoMov,tokenId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletes all movements associated with the specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteAllMovements/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAllMovements(int OrgId)
        {
            try
            {
                int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
                await _movementService.DeleteAllMovements(OrgId, tokenId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
