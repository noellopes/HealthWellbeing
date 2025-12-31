using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomReservationHistory
    {
        public int RoomReservationHistoryId { get; set; }

        public int RoomReservationId { get; set; }
        public int RoomId { get; set; }

        [Required(ErrorMessage = "O campo Id da Consulta é obrigatório.")]
        public int ConsultationId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string ResponsibleName { get; set; }
        public string FinalStatus { get; set; } // "Realizada" ou "Cancelada"

        public DateTime RecordedAt { get; set; } = DateTime.Now;

        public string? Notes { get; set; }

        public Room Room { get; set; }
        public Consultation Consultation { get; set; }
    }
}
