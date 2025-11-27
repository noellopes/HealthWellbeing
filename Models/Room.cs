using HealthWellbeingRoom.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "O tipo de sala é obrigatório.")]
        public int? RoomTypeId { get; set; }

        // Propriedade de navegação anulável
        public RoomType? RoomType { get; set; }

        [NotMapped]
        public string FormattedRoomId => RoomId.ToString("D3");

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(100)]

        public required string Name { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        public int RoomLocationId { get; set; }

        public RoomLocation? RoomLocation { get; set; }

        [Required(ErrorMessage = "A hora de abertura é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "A hora de encerramento é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan ClosingTime { get; set; }

        public int? RoomStatusId  { get; set; }

        public RoomStatus? RoomStatus { get; set; }

        public string? Notes { get; set; }

        public ICollection<LocationMedDevice>? LocalizacaoDispMedicoMovel { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }
    }
}
