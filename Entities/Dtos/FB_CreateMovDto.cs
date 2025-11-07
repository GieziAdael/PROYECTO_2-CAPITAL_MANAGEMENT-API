using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    /// <summary>
    /// Data Transfer Object for creating a financial movement.
    /// </summary>
    public class FB_CreateMovDto
    {
        /// <summary>
        /// Property representing the title of the movement.
        /// </summary>
        public required string TitleMov { get; set; }
        /// <summary>
        /// Property representing the description of the movement.
        /// </summary>
        public required string DescriptMov { get; set; }
        /// <summary>
        /// Property representing the type of the movement.
        /// </summary>
        public required string TypeMov { get; set; }
        /// <summary>
        /// Property representing the amount of the movement.
        /// </summary>
        public decimal AmountMov { get; set; }
    }
}
