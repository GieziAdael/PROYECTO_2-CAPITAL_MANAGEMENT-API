using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    /// <summary>
    /// Entity representing an Employee within an organization.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Property representing the unique identifier of the employee.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Property representing the role of the employee within the organization.
        /// </summary>
        [Required]
        [StringLength(6)] //Owner (5), Admin (5), Viewer (6)
        public required string Role { get; set; }

        /// <summary>
        /// Property representing the foreign key to the User entity.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Property representing the associated User entity.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Property representing the foreign key to the Organization entity.
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// Property representing the associated Organization entity.
        /// </summary>
        public Organization? Organization { get; set; }
    
    }
}
