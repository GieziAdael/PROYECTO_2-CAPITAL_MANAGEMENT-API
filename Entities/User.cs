using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    /// <summary>
    /// Entity representing a User in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Property representing the unique identifier of the user.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Property representing the normalized email of the user.
        /// </summary>
        [Required]
        [StringLength(350)]
        public required string EmailNormalized { get; set; }
        /// <summary>
        /// Property representing the password hash of the user.
        /// </summary>
        [Required]
        public required string PasswordHash { get; set; }
    }
}
