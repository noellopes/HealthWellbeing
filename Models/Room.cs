using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [NotMapped]
        public string FormattedRoomId => RoomId.ToString("D3");

        [Required(ErrorMessage = "O tipo de sala é obrigatório.")]
        public int? RoomTypeId { get; set; }
        //public RoomType? RoomType { get; set; }

        public int? SpecialtyId { get; set; }
        //public Specialty? Specialty { get; set; }

        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        public int RoomLocationId { get; set; }
        //public RoomLocation? RoomLocation { get; set; }

        [Required(ErrorMessage = "A hora de abertura é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "A hora de encerramento é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan ClosingTime { get; set; }

        public int? RoomStatusId { get; set; }
        //public RoomStatus? RoomStatus { get; set; }

        public string? Notes { get; set; }

        //public ICollection<LocationMedDevice> LocalizacaoDispMedicoMovel { get; set; } = new List<LocationMedDevice>();
        //public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
        //public ICollection<RoomConsumable> RoomConsumables { get; set; } = new List<RoomConsumable>();
        //public ICollection<RoomReservation> RoomReservations { get; set; } = new List<RoomReservation>();

        // Nova navegação: uma sala pode ter muitas consultas
        //public ICollection<Consultation>? Consultations { get; set; }
    }
}
