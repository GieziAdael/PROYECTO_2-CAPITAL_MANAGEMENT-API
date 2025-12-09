using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities
{
    /// <summary>
    /// Entity representing a financial movement within an organization.
    /// </summary>
    public class Movement
    {
        /// <summary>
        /// Property representing the unique identifier for the movement.
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Property representing the movement number.
        /// </summary>
        public int NoMov {  get; set; }
        /// <summary>
        /// Property representing the title of the movement.
        /// </summary>
        [Required]
        [StringLength(80)]
        public required string TitleMov { get; set; }
        /// <summary>
        /// Property representing the description of the movement.
        /// </summary>
        [Required]
        [StringLength(250)]
        public string? DescriptMov { get; set; }
        /// <summary>
        /// Property representing the type of the movement (Ingreso or Egreso).
        /// </summary>
        [Required]
        [StringLength(7)] //Ingreso (7), Egreso (6)
        public required string TypeMov { get; set; }
        /// <summary>
        /// Property representing the date of the movement.
        /// </summary>
        public DateTime DateMov { get; set; }
        /// <summary>
        /// Property representing the amount of the movement.
        /// </summary>
        [Precision(18,2)]
        public decimal AmountMov { get; set; }

        /// <summary>
        /// Property representing the foreign key to the Organization entity.
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// Property representing the associated Organization entity.
        /// </summary>
        public Organization? Organization { get; set; }
    }
}
