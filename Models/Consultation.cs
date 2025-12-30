using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class Consultation
    {
        [Key]
        public int ConsultationId { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime ConsultationDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Status { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string? PatientName { get; set; }

        [ForeignKey(nameof(Specialty))]
        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
    }


}