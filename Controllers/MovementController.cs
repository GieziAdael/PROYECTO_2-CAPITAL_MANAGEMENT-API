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

        /// <summary>
        /// Initializes a new instance of the <see cref="MovementController"/> class.
        /// </summary>
        /// <param name="movementRepo">The repository used to manage movement-related data. This parameter cannot be <see langword="null"/>.</param>
        /// <param name="employeeRepo">The repository used to manage employee-related data. This parameter cannot be <see langword="null"/>.</param>
        /// <param name="mapper">The object mapper used to map between domain models and DTOs. This parameter cannot be <see
        /// langword="null"/>.</param>
        public MovementController(IMovementRepo movementRepo, IEmployeeRepo employeeRepo, IMapper mapper)
        {
            _movementRepo = movementRepo;
            _employeeRepo = employeeRepo;
            _mapper = mapper;
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
            //Validations
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para ver los movimientos de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin" && role != "Viewer")
                return BadRequest("No tienes permisos para ver los movimientos de esta organización");

            //Actions
            var movements = await _movementRepo.GetAllMovements(OrgId);
            var movementsDto = _mapper.Map<IEnumerable<Movement>>(movements);
            return Ok(movementsDto);
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
            //Validations
            if (string.IsNullOrEmpty(createMovDto.TitleMov) || string.IsNullOrEmpty(createMovDto.TypeMov))
                return BadRequest("Hay campos vacios");
            if (createMovDto.AmountMov < 0)
                return BadRequest("El monto no puede ser negativo");
            if (createMovDto.TypeMov != "Ingreso" && createMovDto.TypeMov != "Egreso")
                return BadRequest("El tipo de movimiento debe ser 'Ingreso' o 'Egreso'");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                return BadRequest("No tienes permisos para crear movimientos en esta organización");
            //Actions
            var movenets = await _movementRepo.GetAllMovements(OrgId);

            var register = new Movement
            {
                NoMov = movenets.Count() + 1,
                TitleMov = createMovDto.TitleMov,
                DescriptMov = createMovDto.DescriptMov,
                TypeMov = createMovDto.TypeMov,
                DateMov = DateTime.UtcNow,
                AmountMov = createMovDto.AmountMov,
                OrganizationId = OrgId
            };
            var movement = _mapper.Map<Movement>(register);
            if (!await _movementRepo.NewMovement(movement))
                return BadRequest("Error al crear el movimiento");
            return CreatedAtAction(nameof(CreateMovement), new { id = movement.Id }, movement);
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
            //Validations
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                return BadRequest("No tienes acceso para ver los movimientos de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin" && role != "Viewer")
                return BadRequest("No tienes permisos para ver los movimientos de esta organización");
            //Actions
            var movements = await _movementRepo.GetAllMovements(OrgId);
            decimal balance = 0;
            foreach (var mov in movements)
            {
                if (mov.TypeMov == "Ingreso")
                {
                    balance += mov.AmountMov;
                }
                else if (mov.TypeMov == "Egreso")
                {
                    balance -= mov.AmountMov;
                }
            }
            return Ok(new { Balance = balance });
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
            //Validations
            if (string.IsNullOrEmpty(fB_UpdateMovDto.TitleMov) || string.IsNullOrEmpty(fB_UpdateMovDto.TypeMov))
                return BadRequest("Hay campos vacios");
            if (fB_UpdateMovDto.AmountMov < 0)
                return BadRequest("El monto no puede ser negativo");
            if (fB_UpdateMovDto.TypeMov != "Ingreso" && fB_UpdateMovDto.TypeMov != "Egreso")
                return BadRequest("El tipo de movimiento debe ser 'Ingreso' o 'Egreso'");
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                return BadRequest("No tienes permisos para actualizar movimientos en esta organización");

            var movement = await _movementRepo.GetMovByNOrg(OrgId, NoMov);
            if (movement == null)
                return BadRequest("El movimiento no existe");
            //Actions
            _mapper.Map<Movement>(movement);
            movement.Id = movement.Id;
            movement.TitleMov = fB_UpdateMovDto.TitleMov;
            movement.DescriptMov = fB_UpdateMovDto.DescriptMov;
            movement.TypeMov = fB_UpdateMovDto.TypeMov;
            movement.AmountMov = fB_UpdateMovDto.AmountMov;

            if (!await _movementRepo.PatchMov(movement))
                return BadRequest("Error al actualizar el movimiento");
            return Ok(movement);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMovement(int OrgId, int NoMov)
        {
            //Validations
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                return BadRequest("No tienes permisos para eliminar movimientos en esta organización");
            var movement = await _movementRepo.GetMovByNOrg(OrgId, NoMov);
            if (movement == null)
                return BadRequest("El movimiento no existe");
            //Actions
            if (!await _movementRepo.DeleteMov(movement))
                return BadRequest("Error al eliminar el movimiento");

            var movements = await _movementRepo.GetAllMovements(OrgId);
            int counter = 1;
            foreach (var mov in movements)
            {
                mov.NoMov = counter;
                await _movementRepo.PatchMov(mov);
                counter++;
            }

            return Ok("Movimiento eliminado correctamente");
        }

        /// <summary>
        /// Deletes all movements associated with the specified organization.
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("DeleteAllMovements/{OrgId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAllMovements(int OrgId)
        {
            //Validations
            int tokenId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner")
                return BadRequest("Solo el Owner puede eliminar todos los movimientos en esta organización");
            var movements = await _movementRepo.GetAllMovements(OrgId);
            if (movements == null || !movements.Any())
                return BadRequest("No hay movimientos para eliminar en esta organización");
            //Actions
            foreach (var movement in movements)
            {
                if (!await _movementRepo.DeleteMov(movement))
                    return BadRequest("Error al eliminar los movimientos");
            }
            return Ok("Todos los movimientos han sido eliminados correctamente");
        }
    }
}
