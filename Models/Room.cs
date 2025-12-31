using HealthWellbeingRoom.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Room
    {
        // Identificador único da sala
        [Key]
        public int RoomId { get; set; }
        [NotMapped]
        public string FormattedRoomId => RoomId.ToString("D3");


        // Tipo de sala
        [Required(ErrorMessage = "O tipo de sala é obrigatório.")]
        public int? RoomTypeId { get; set; }
        public RoomType? RoomType { get; set; }


        // Especialidade associada à sala
        public int? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }


        // Nome da sala
        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(100)]

        public required string Name { get; set; }


        // Localização da sala
        [Required(ErrorMessage = "A localização é obrigatória.")]
        public int RoomLocationId { get; set; }
        public RoomLocation? RoomLocation { get; set; }


        // Horário de abertura da sala
        [Required(ErrorMessage = "A hora de abertura é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan OpeningTime { get; set; }


        // Horário de encerramento da sala
        [Required(ErrorMessage = "A hora de encerramento é obrigatória.")]
        [DataType(DataType.Time)]
        public TimeSpan ClosingTime { get; set; }


        // Status da sala
        public int? RoomStatusId  { get; set; }

        public RoomStatus? RoomStatus { get; set; }


        // Comentários adicionais sobre a sala
        public string? Notes { get; set; }


        // Propriedades de navegação para relacionamentos
        public ICollection<LocationMedDevice>? LocalizacaoDispMedicoMovel { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }
    }
}
