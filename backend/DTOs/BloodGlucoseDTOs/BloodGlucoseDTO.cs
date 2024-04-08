namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseDto
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTimeRecorded { get; set; }

        public int UserId { get; set; }
    }
}