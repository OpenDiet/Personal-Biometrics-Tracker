using System.ComponentModel.DataAnnotations;

namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseAddDto
    {
        [Required]
        public decimal? Value { get; set; }

        [Required]
        public DateTime? DateTimeRecorded { get; set; }
    }
}