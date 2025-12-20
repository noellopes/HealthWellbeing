using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace HealthWellbeingRoom.Models
{
    public class Consultation
    {
        public int ConsultationId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime ConsultationDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Status {  get; set; }
        public string DoctorName { get; set; } = string.Empty;

        [ForeignKey(nameof(Specialty))]
        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }


        // Inicializado para evitar NullReferenceException (NRE)
        public ICollection<ConsultationDevice> ConsultationDevices { get; set; } = new List<ConsultationDevice>();
        public ICollection<ConsultationConsumable> ConsultationConsumables { get; set; } = new List<ConsultationConsumable>();

    }

    public class ConsultationDevice
    {
        // Chave estrangeira para a Consulta
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;

        // Chave estrangeira para o Dispositivo Médico
        public int DeviceId { get; set; }
        public MedicalDevice Device { get; set; } = null!;
    }

    public class ConsultationConsumable
    {
        // Chave estrangeira para a Consulta
        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;

        // Chave estrangeira para o Consumível
        public int ConsumableId { get; set; }
        public Consumivel Consumavel { get; set; } = null!;
    }
}
