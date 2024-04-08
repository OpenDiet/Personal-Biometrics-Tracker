namespace PersonalBiometricsTracker.Dtos
{
    public class WeightAddDto
    {
        public decimal Value { get; set; }

        public DateTime DateRecorded { get; set; }

        public int UserId { get; set; }
    }
}