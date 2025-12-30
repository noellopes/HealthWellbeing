using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class RoomReservation
    {
        [Key]
        public int RoomReservationId { get; set; }

        //Responsável
        [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
        [StringLength(100)]
        public string ResponsibleName { get; set; } = string.Empty;


        //Sala
        [Required(ErrorMessage = "A sala é obrigatória.")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }   // Navegação para entidade Room

        //Consulta
        public int ConsultationId { get; set; }
        public Consultation? Consultation { get; set; }

        //Paciente
        public int? PatientId { get; set; }

        //Especialidade
        public int? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        //Data, Hora e duracao
        [Required(ErrorMessage = "A data/hora de início é obrigatória.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "A data/hora de fim é obrigatória.")]
        public DateTime EndTime { get; set; }

        [NotMapped]
        public TimeSpan Duration => EndTime - StartTime;

        //Recursos
        public ICollection<RoomConsumable>? Consumables { get; set; }
        public ICollection<LocationMedDevice>? MedicalDevices { get; set; }

        //Estado da reserva
        [StringLength(50)]
        public string Status { get; set; } = "Pendente";

        //Observações adicionais
        public string? Notes { get; set; }
    }
}