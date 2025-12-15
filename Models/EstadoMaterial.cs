using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class EstadoMaterial
    {
        [Key]
        public int MaterialStatusId { get; set; }


        [Required(ErrorMessage = "O nome do status é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do status não pode ultrapassar 50' caracteres.")]

        public string Nome { get; set; }   // Example: "In Use", "Available", "Damaged"

        public string Descricao { get; set; }  // Optional
    }
}
