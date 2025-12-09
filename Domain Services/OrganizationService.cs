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
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IMovementRepo _movementRepo;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationRepo"></param>
        /// <param name="employeeRepo"></param>
        /// <param name="movementRepo"></param>
        /// <param name="mapper"></param>
        public OrganizationService(IOrganizationRepo organizationRepo, IEmployeeRepo employeeRepo, 
            IMovementRepo movementRepo, IMapper mapper)
        {
            _organizationRepo = organizationRepo;
            _employeeRepo = employeeRepo;
            _movementRepo = movementRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FB_ListOrganizationsDto>> MyOrganizations(int tokenId)
        {
            var organizations = await _organizationRepo.GetAllMyOrginations(tokenId);
            var organizationsDto = _mapper.Map<List<FB_ListOrganizationsDto>>(
                organizations,
                opt => opt.Items["UserId"] = tokenId
            );

            return organizationsDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FB_ListOrganizationsDto>> OrganizationsAfilied(int tokenId)
        {
            var organizations = await _organizationRepo.GetAllOthersOrganizationsByOrgId(tokenId);
            var organizationsDto = _mapper.Map<List<FB_ListOrganizationsDto>>(
                organizations,
                opt => opt.Items["UserId"] = tokenId
            );
            return organizationsDto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createOrgDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Organization> CreateOrganization(FB_CreateOrgDto createOrgDto, int tokenId)
        {
            //Rules and validations
            //Name or Password cannot be empty
            if (string.IsNullOrEmpty(createOrgDto.NameOrganization) || string.IsNullOrEmpty(createOrgDto.PasswordOrganization))
                throw new Exception("El Nombre y/o Password son requeridos");
            //Exists Name
            if (await _organizationRepo.ExistsOrgByName(createOrgDto.NameOrganization))
                throw new Exception($"El Nombre '{createOrgDto.NameOrganization}' ya se encuentra en uso");
            //Password must be longer than 7 characters
            if (createOrgDto.PasswordOrganization.Length < 8)
                throw new Exception("La contraseña debe tener al menos 8 caracteres");
            //The Name must be less than 150 characters
            if (createOrgDto.NameOrganization.Length > 150)
                throw new Exception("El Nombre no debe exceder los 150 caracteres");

            //Actions
            //Create Organization

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
                throw new Exception("Error al registrar la Organizacion");
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

            return register;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fB_LoginOrgDto"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Organization> LoginOrganization(FB_CreateOrgDto fB_LoginOrgDto, int tokenId)
        {
            //Rules and validations
            if (string.IsNullOrEmpty(fB_LoginOrgDto.NameOrganization) || string.IsNullOrEmpty(fB_LoginOrgDto.PasswordOrganization))
                throw new Exception("El Nombre y Password son requeridos");

            var organization = await _organizationRepo.GetOrganization(fB_LoginOrgDto.NameOrganization, fB_LoginOrgDto.PasswordOrganization);
            if (organization == null)
                throw new Exception("Nombre o contraseña incorrectos");
            //Actions


            var employee = await _employeeRepo.IsMember(tokenId, organization.Id);

            if (!employee)
                throw new Exception("No eres miembro de esta organización");

            return organization;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="newPassword"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdatePasswordOrganization(int orgId, string newPassword, int tokenId)
        {
            //Rules and validations
            if (orgId <= 0)
                throw new Exception("El Id de la organización es inválido");
            if (string.IsNullOrEmpty(newPassword))
                throw new Exception("La nueva contraseña es requerida");
            if (newPassword.Length < 8)
                throw new Exception("La nueva contraseña debe tener al menos 8 caracteres");
            if (!await _organizationRepo.ExistsOrgById(orgId))
                throw new Exception("La organización no existe");

            //Is the user owner of the organization?
            var employee = await _employeeRepo.GetByIdUserEmployee(tokenId, orgId);
            if (employee == null)
                throw new Exception("No se encontro dicha organizacion");
            if (employee.Role != "Owner")
                throw new Exception("No tienes permisos para actualizar la contraseña de esta organización");

            //Actions
            var registro = await _organizationRepo.GetOrganizationById(orgId);
            if (registro == null)
                throw new Exception("La organización no existe");

            var organization = _mapper.Map<Organization>(registro);
            organization.Id = orgId;
            organization.PasswordOrgHash = newPassword;

            if (!await _organizationRepo.PatchOrg(organization))
                throw new Exception("Error al actualizar la contraseña de la organización");

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteOrganization(int orgId, int tokenId)
        {
            //Rules and validations
            if (orgId <= 0)
                throw new Exception("El Id de la organización es inválido");
            if (!await _organizationRepo.ExistsOrgById(orgId))
                throw new Exception("La organización no existe");
            //Is the user owner of the organization?
            var employee = await _employeeRepo.GetByIdUserEmployee(tokenId, orgId);
            if (employee == null)
                throw new Exception("No se encontro el empleado");
            if (employee.Role != "Owner")
                throw new Exception("No tienes permisos para eliminar esta organización");
            //Actions
            var registro = await _organizationRepo.GetOrganizationById(orgId);
            if (registro == null)
                throw new Exception("La organización no existe");
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
                throw new Exception("Error al eliminar la organización");
        }
    }
}
