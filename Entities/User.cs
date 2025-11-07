using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(350)]
        public required string EmailNormalized { get; set; }
        [Required]
        public required string PasswordHash { get; set; }
    }
}
