namespace PersonalBiometricsTracker.DTOs
{
    public class WeightDTO
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public DateTime DateRecorded { get; set; }

        public int UserId { get; set; }
    }
}