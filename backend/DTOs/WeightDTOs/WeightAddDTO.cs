using System.ComponentModel.DataAnnotations;

namespace PersonalBiometricsTracker.Dtos
{
    public class WeightAddDto
    {
        [Required]
        public decimal? Value { get; set; }

        [Required]
        public DateTime? DateRecorded { get; set; }
    }
}