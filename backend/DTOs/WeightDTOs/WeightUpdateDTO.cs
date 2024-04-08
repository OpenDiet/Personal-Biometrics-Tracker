namespace PersonalBiometricsTracker.Dtos
{
    public class WeightUpdateDto
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateRecorded { get; set; }
    }
}