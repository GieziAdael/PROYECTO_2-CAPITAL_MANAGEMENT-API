using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    public class FB_CreateUserDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
