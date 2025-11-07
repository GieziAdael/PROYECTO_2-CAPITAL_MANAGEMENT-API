using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for User entity.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Property representing the unique identifier of the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Property representing the email of the user.
        /// </summary>
        public required string EmailNormalized { get; set; }
        /// <summary>
        /// Property representing the password hash of the user.
        /// </summary>
        public required string PasswordHash { get; set; }
    }
}
