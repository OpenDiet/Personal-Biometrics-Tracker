using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBiometricsTracker.Entities
{
    public class BloodGlucose
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Value { get; set; }

        [Required]
        public DateTime DateTimeRecorded { get; set; }

        // Foreign key reference to the User entity
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}