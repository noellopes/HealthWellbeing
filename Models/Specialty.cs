using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class Specialty
    {
        // Identificador único da especialidade
        [Key]
        public int SpecialtyId { get; set; }

        // Nome da especialidade
        //[Required(ErrorMessage = "O nome da especialidade é obrigatório.")]
        [StringLength(100, ErrorMessage = "Maximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        // Descrição da especialidade
        public string? Description { get; set; }


        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
