using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class Consultation
    {
        [Key]
        public int ConsultationId { get; set; }

        [ForeignKey(nameof(Room))]
        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        public DateTime BookingDate { get; set; }
        // Data da consulta, vem da reserva de sala
        public DateTime? ConsultationDate { get; set; }
        // Hora de início, vem da reserva de sala
        public TimeOnly? StartTime { get; set; }
        // Hora de fim, vem da reserva de sala
        public TimeOnly? EndTime { get; set; }

        public string? Status { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string? PatientName { get; set; }

        [ForeignKey(nameof(Specialty))]
        public int SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }
    }


}