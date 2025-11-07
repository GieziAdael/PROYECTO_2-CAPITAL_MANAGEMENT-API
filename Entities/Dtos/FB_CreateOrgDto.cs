namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for creating a new organization.
    /// </summary>
    public class FB_CreateOrgDto
    {
        /// <summary>
        /// Property representing the name of the organization.
        /// </summary>
        public string? NameOrganization { get; set; }
        /// <summary>
        /// Property representing the password for the organization.
        /// </summary>
        public string? PasswordOrganization { get; set; }
    }
}
