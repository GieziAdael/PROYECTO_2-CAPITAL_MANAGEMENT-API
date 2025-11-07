using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public required string NameOrg { get; set; }
        public required string PasswordOrgHash { get; set; }
        
        //FK ID User
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
