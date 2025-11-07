using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API_CAPITAL_MANAGEMENT.Entities.Dtos
{
    public class FB_CreateMovDto
    {
        public required string TitleMov { get; set; }
        public required string DescriptMov { get; set; }
        public required string TypeMov { get; set; }
        public decimal AmountMov { get; set; }
    }
}
