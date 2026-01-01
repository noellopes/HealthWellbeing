using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomReservation
    {
        [Key]
        public int RoomReservationId { get; set; }

        // Responsável
        [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
        [StringLength(100)]
        public string ResponsibleName { get; set; } = string.Empty;

        // Sala
        [Required(ErrorMessage = "A sala é obrigatória.")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // Consulta
        public int ConsultationId { get; set; }
        public Consultation? Consultation { get; set; }

        // Paciente
        public int? PatientId { get; set; }

        // Especialidade
        public int? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        // Data da consulta (AGORA MAPEADA)
        [Required(ErrorMessage = "A data da consulta é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime ConsultationDate { get; set; }


        [Required(ErrorMessage = "A hora de início é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan StartHour { get; set; }

        [Required(ErrorMessage = "A hora de fim é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan EndHour { get; set; }

        // Duração calculada
        public TimeSpan? Duration => EndHour - StartHour;

        // Recursos
        public ICollection<RoomConsumable>? Consumables { get; set; }
        public ICollection<LocationMedDevice>? MedicalDevices { get; set; }

        // Estado
        [StringLength(50)]
        public string Status { get; set; } = "Pendente";

        // Observações
        public string? Notes { get; set; }
    }
}