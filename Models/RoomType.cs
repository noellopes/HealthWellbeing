using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de sala é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do tipo de sala não pode exceder 100 caracteres.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Relação com salas
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}