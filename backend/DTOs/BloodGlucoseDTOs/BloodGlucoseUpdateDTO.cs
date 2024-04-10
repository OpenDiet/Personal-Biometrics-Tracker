using Microsoft.Extensions.Configuration.UserSecrets;

namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseUpdateDto
    {
        public decimal? Value { get; set; }

        public DateTime? DateTimeRecorded { get; set; }

    }
}