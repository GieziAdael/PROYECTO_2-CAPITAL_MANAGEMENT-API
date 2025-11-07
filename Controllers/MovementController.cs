using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_CAPITAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementController : ControllerBase
    {
        private readonly IMovementRepo _movementRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMapper _mapper;

        public MovementController(IMovementRepo movementRepo, IEmployeeRepo employeeRepo, IMapper mapper)
        {
            _movementRepo = movementRepo;
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("MyMovements/{OrgId:int}")]
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

        [Authorize]
        [HttpGet("CalculateBalance/{OrgId:int}")]
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
