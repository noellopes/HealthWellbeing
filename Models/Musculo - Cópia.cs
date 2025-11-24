using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Musculo
    {

        public int MusculoId { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome do músculo é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome do músculo não pode exceder 150 caracteres.")]
        [Display(Name = "Nome do Músculo")]
        public string Nome_Musculo { get; set; }

        // Chave estrangeira — Um músculo pertence a um grupo muscular
        [Required(ErrorMessage = "É necessário associar o músculo a um grupo muscular.")]
        [Display(Name = "Grupo Muscular")]
        public int GrupoMuscularId { get; set; }

        [ForeignKey("GrupoMuscularId")]
        public GrupoMuscular? GrupoMuscular { get; set; }
    }
}
