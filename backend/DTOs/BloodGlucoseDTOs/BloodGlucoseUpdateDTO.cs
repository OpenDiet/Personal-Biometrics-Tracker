using Microsoft.Extensions.Configuration.UserSecrets;

namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseUpdateDto
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTimeRecorded { get; set; }

        public int UserId { get; set; }
    }
}