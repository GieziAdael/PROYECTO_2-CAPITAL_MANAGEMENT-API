using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for creating a user via Firebase.
    /// </summary>
    public class FB_CreateUserDto
    {
        /// <summary>
        /// Property representing the email of the user.
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Property representing the password of the user.
        /// </summary>
        public string? Password { get; set; }
    }
}
