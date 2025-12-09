using API_CAPITAL_MANAGEMENT.Domain_Services.IServices;
using API_CAPITAL_MANAGEMENT.Entities;
using API_CAPITAL_MANAGEMENT.Entities.Dtos;
using API_CAPITAL_MANAGEMENT.Repositories.IRepositories;
using AutoMapper;
using System.Security.Cryptography;

namespace API_CAPITAL_MANAGEMENT.Domain_Services
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeRepo"></param>
        /// <param name="mapper"></param>
        public EmployeeService(IEmployeeRepo employeeRepo, IMapper mapper)
        {
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
        public async Task<IEnumerable<Employee>> AllMembersAOrganization(int OrgId, int tokenId)
        {
            //Validations
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para ver los miembros de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner" && role != "Admin" && role != "Viewer")
                throw new Exception("No tienes permisos para ver los miembros de esta organización");
            //Actions
            // Assuming there's a method to get members by organization ID
            var members = await _employeeRepo.GetMembersByOrganizationId(OrgId);
            var membersDto = _mapper.Map<List<Employee>>(members);

            return membersDto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="addMemberDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Employee> AddMember(int OrgId, FB_CreateEmployeeDto addMemberDto, int tokenId)
        {
            //Validations
            if (string.IsNullOrEmpty(addMemberDto.EmailUser) || string.IsNullOrEmpty(addMemberDto.Role))
                throw new Exception("Hay campos vacios");
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para agregar miembros a esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner")
                throw new Exception("Solo el Owner puede agregar miembros a esta organización");
            if (await _employeeRepo.ExistsByEmailToOrg(OrgId, addMemberDto.EmailUser))
                throw new Exception("El usuario ya es miembro de esta organización");
            if (addMemberDto.Role != "Admin" && addMemberDto.Role != "Viewer")
                throw new Exception("El rol debe ser 'Admin' o 'Viewer'");
            //Actions
            var user = await _employeeRepo.GetByIdUserEmployee(addMemberDto.EmailUser);
            if (user == null)
                throw new Exception("El usuario con ese correo no existe");

            var newEmployee = new Employee
            {
                Role = addMemberDto.Role,
                UserId = user.Id,
                OrganizationId = OrgId
            };
            var register = _mapper.Map<Employee>(newEmployee);
            if (!await _employeeRepo.NewEmployeeToOrg(register))
                throw new Exception("Error al agregar el miembro a la organización");

            return register;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="updateRoleDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task UpdateRoleMember(int OrgId, FB_CreateEmployeeDto updateRoleDto, int tokenId)
        {
            //Validations
            if (string.IsNullOrEmpty(updateRoleDto.EmailUser) || string.IsNullOrEmpty(updateRoleDto.Role))
                throw new Exception("Hay campos vacios");
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para actualizar roles en esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner")
                throw new Exception("Solo el Owner puede actualizar roles en esta organización");
            if (updateRoleDto.Role != "Admin" && updateRoleDto.Role != "Viewer")
                throw new Exception("El rol debe ser 'Admin' o 'Viewer'");


            //Actions
            var user = await _employeeRepo.GetByIdUserEmployee(updateRoleDto.EmailUser);
            if (user == null)
                throw new Exception("El usuario con ese correo no existe");
            if (tokenId == user.Id)
                throw new Exception("El Owner no puede cambiar su propio rol");

            var employee = await _employeeRepo.BodyByEmailToOrg(OrgId, updateRoleDto.EmailUser);
            var register = _mapper.Map<Employee>(employee);
            register.Id = employee.Id;
            register.Role = updateRoleDto.Role;

            if (!await _employeeRepo.PatchRoleEmployeeToOrg(register))
                throw new Exception("Error al actualizar el rol del miembro");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrgId"></param>
        /// <param name="email"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task RemoveMember(int OrgId, string email, int tokenId)
        {
            //Validations
            if (string.IsNullOrEmpty(email))
                throw new Exception("El correo no puede estar vacío");
            if (!await _employeeRepo.IsMember(tokenId, OrgId))
                throw new Exception("No tienes acceso para eliminar miembros de esta organización");
            string role = await _employeeRepo.WhatThisRole(tokenId, OrgId);
            if (role != "Owner")
                throw new Exception("Solo el Owner puede eliminar miembros de esta organización");

            //Actions
            var user = await _employeeRepo.BodyByEmailToOrg(OrgId, email);
            if (user == null)
                throw new Exception("El usuario no es miembro de esta organización");
            if (user.UserId == tokenId)
                throw new Exception("El Owner no puede eliminarse a sí mismo");

            var register = _mapper.Map<Employee>(user);
            if (!await _employeeRepo.DeleteEmployeeToOrg(register))
                throw new Exception("Error al eliminar el miembro de la organización");
        }
    }
}
