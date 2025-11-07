using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(6)] //Owner (5), Admin (5), Viewer (6)
        public required string Role { get; set; }
        
        //FK ID User
        public int UserId { get; set; }
        public User? User { get; set; }

        //FK ID Organization
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    
    }
}
