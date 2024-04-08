namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseUpdateDto
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTimeRecorded { get; set; }
    }
}