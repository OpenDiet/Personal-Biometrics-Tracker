namespace PersonalBiometricsTracker.DTOs
{
    public class BloodGlucoseDTO
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTimeRecorded { get; set; }

        public int UserId { get; set; }
    }
}