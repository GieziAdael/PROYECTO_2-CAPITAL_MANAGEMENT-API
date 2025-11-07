using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for user login.
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// Property representing the email of the user.
        /// </summary>
        [Required(ErrorMessage = "Se requiere el email")]
        public required string Email { get; set; }
        /// <summary>
        /// Property representing the password of the user.
        /// </summary>
        [Required(ErrorMessage = "Se requiere el password")]
        public required string Password { get; set; }
    }
}
