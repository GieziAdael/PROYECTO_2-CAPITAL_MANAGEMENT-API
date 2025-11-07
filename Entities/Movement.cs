using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    public class Movement
    {
        [Key]
        public int Id { get; set; }
        public int NoMov {  get; set; }
        [Required]
        [StringLength(80)]
        public required string TitleMov { get; set; }
        [Required]
        [StringLength(250)]
        public required string DescriptMov { get; set; }
        [Required]
        [StringLength(7)] //Ingreso (7), Egreso (6)
        public required string TypeMov { get; set; }
        public DateTime DateMov { get; set; }
        [Precision(18,2)]
        public decimal AmountMov { get; set; }

        //FK ID Organization
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }
}
