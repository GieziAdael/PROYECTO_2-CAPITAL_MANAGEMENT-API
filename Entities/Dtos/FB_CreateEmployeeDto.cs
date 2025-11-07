namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for creating a new employee in an organization.
    /// </summary>
    public class FB_CreateEmployeeDto
    {
        /// <summary>
        /// Property representing the email of the user to be added as an employee.
        /// </summary>
        public required string EmailUser { get; set; }
        /// <summary>
        /// Property representing the role assigned to the new employee.
        /// </summary>
        public required string Role { get; set; }
    }
}
