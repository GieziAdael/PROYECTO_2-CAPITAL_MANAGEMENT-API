using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string EmailNormalized { get; set; }
        public required string PasswordHash { get; set; }
    }
}
