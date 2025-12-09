using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;

namespace API_CAPITAL_MANAGEMENT.Domain_Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MovementService : IMovementService
    {
        private readonly IMovementRepo _movementRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movementRepo"></param>
        /// <param name="employeeRepo"></param>
        /// <param name="mapper"></param>
        public MovementService(IMovementRepo movementRepo, IEmployeeRepo employeeRepo, IMapper mapper)
        {
            _movementRepo = movementRepo;
            _employeeRepo = employeeRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<Movement>> MyMovements(int OrgId, int tokenId)
        {
            //Validations
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para ver los movimientos de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin" && role != "Viewer")
                throw new Exception("No tienes permisos para ver los movimientos de esta organización");

            //Actions
            var movements = await _movementRepo.GetAllMovements(OrgId);
            var movementsDto = _mapper.Map<IEnumerable<Movement>>(movements);

            return movementsDto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="createMovDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Movement> CreateMovement(int OrgId, FB_CreateMovDto createMovDto, int tokenId)
        {
            //Validations
            if (string.IsNullOrEmpty(createMovDto.TitleMov) || string.IsNullOrEmpty(createMovDto.TypeMov))
                throw new Exception("Hay campos vacios");
            if (createMovDto.AmountMov < 0)
                throw new Exception("El monto no puede ser negativo");
            if (createMovDto.TypeMov != "Ingreso" && createMovDto.TypeMov != "Egreso")
                throw new Exception("El tipo de movimiento debe ser 'Ingreso' o 'Egreso'");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                throw new Exception("No tienes permisos para crear movimientos en esta organización");
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
                throw new Exception("Error al crear el movimiento");

            return movement;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<decimal> CalculateBalance(int OrgId, int tokenId)
        {
            //Validations
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para ver los movimientos de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin" && role != "Viewer")
                throw new Exception("No tienes permisos para ver los movimientos de esta organización");
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
            return balance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="NoMov"></param>
        /// <param name="fB_UpdateMovDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Movement> UpdateMovement(int OrgId, int NoMov, FB_CreateMovDto fB_UpdateMovDto, int tokenId)
        {
            //Validations
            if (string.IsNullOrEmpty(fB_UpdateMovDto.TitleMov) || string.IsNullOrEmpty(fB_UpdateMovDto.TypeMov))
                throw new Exception("Hay campos vacios");
            if (fB_UpdateMovDto.AmountMov < 0)
                throw new Exception("El monto no puede ser negativo");
            if (fB_UpdateMovDto.TypeMov != "Ingreso" && fB_UpdateMovDto.TypeMov != "Egreso")
                throw new Exception("El tipo de movimiento debe ser 'Ingreso' o 'Egreso'");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                throw new Exception("No tienes permisos para actualizar movimientos en esta organización");

            var movement = await _movementRepo.GetMovByNOrg(OrgId, NoMov);
            if (movement == null)
                throw new Exception("El movimiento no existe");
            //Actions
            _mapper.Map<Movement>(movement);
            movement.Id = movement.Id;
            movement.TitleMov = fB_UpdateMovDto.TitleMov;
            movement.DescriptMov = fB_UpdateMovDto.DescriptMov;
            movement.TypeMov = fB_UpdateMovDto.TypeMov;
            movement.AmountMov = fB_UpdateMovDto.AmountMov;

            if (!await _movementRepo.PatchMov(movement))
                throw new Exception("Error al actualizar el movimiento");

            return movement;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="NoMov"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteMovement(int OrgId, int NoMov, int tokenId)
        {

            //Validations
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin")
                throw new Exception("No tienes permisos para eliminar movimientos en esta organización");
            var movement = await _movementRepo.GetMovByNOrg(OrgId, NoMov);
            if (movement == null)
                throw new Exception("El movimiento no existe");
            //Actions
            if (!await _movementRepo.DeleteMov(movement))
                throw new Exception("Error al eliminar el movimiento");

            var movements = await _movementRepo.GetAllMovements(OrgId);
            int counter = 1;
            foreach (var mov in movements)
            {
                mov.NoMov = counter;
                await _movementRepo.PatchMov(mov);
                counter++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteAllMovements(int OrgId, int tokenId)
        {
            //Validations
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner")
                throw new Exception("Solo el Owner puede eliminar todos los movimientos en esta organización");
            var movements = await _movementRepo.GetAllMovements(OrgId);
            if (movements == null || !movements.Any())
                throw new Exception("No hay movimientos para eliminar en esta organización");
            //Actions
            foreach (var movement in movements)
            {
                if (!await _movementRepo.DeleteMov(movement))
                    throw new Exception("Error al eliminar los movimientos");
            }
        }
    }
}
