using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    /// <summary>
    /// Entity representing an Organization.
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Property representing the unique identifier of the organization.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Property representing the name of the organization.
        /// </summary>
        [Required]
        [StringLength(150)]
        public required string NameOrg { get; set; }
        /// <summary>
        /// Property representing the hashed password of the organization.
        /// </summary>
        public required string PasswordOrgHash { get; set; }

        /// <summary>
        /// Property representing the foreign key to the User entity.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Property representing the associated User entity.
        /// </summary>
        public User? User { get; set; }
    }
}
