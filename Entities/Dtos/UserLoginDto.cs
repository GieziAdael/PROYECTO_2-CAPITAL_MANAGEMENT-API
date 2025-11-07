using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Se requiere el email")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Se requiere el password")]
        public required string Password { get; set; }
    }
}
