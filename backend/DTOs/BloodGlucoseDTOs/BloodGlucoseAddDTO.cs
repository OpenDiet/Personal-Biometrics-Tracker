namespace PersonalBiometricsTracker.DTOs
{
    public class BloodGlucoseAddDTO
    {
        public decimal Value { get; set; }
        public DateTime DateTimeRecorded { get; set; }

        public int UserId { get; set; }
    }
}