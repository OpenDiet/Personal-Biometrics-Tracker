namespace PersonalBiometricsTracker.Dtos
{
    public class BloodGlucoseUpdateDTO
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateTimeRecorded { get; set; }
    }
}