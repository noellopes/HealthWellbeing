using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }

        [Required]
        public string ConsultationType { get; set; } = string.Empty;

        [Required]
        public string PatientName { get; set; } = string.Empty;

        public string? Doctor { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        public string Status { get; set; } = "Reservada";

        public string? Notes { get; set; }

        // Inicializadas para evitar NRE
        public ICollection<ConsultationDevice> ConsultationDevices { get; set; } = new List<ConsultationDevice>();
        public ICollection<ConsultationConsumable> ConsultationConsumables { get; set; } = new List<ConsultationConsumable>();
    }

    public class ConsultationDevice
    {
        // FK para Consultation
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;

        // FK consistente e nome de navegação coerente com o controller
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;
    }

    public class ConsultationConsumable
    {
        // FK para Consultation
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;

        // FK consistente e nome de navegação coerente com o controller
        public int ConsumableId { get; set; }
        public Consumivel Consumivel { get; set; } = null!;
    }
}
