using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class PlanoExercicios
    {
        public int PlanoExerciciosId { get; set; } // Chave primária

        // Chave estrangeira — Um músculo pertence a um grupo muscular
        [Required(ErrorMessage = "É necessário associar um Utente")]
        [Display(Name = "Utente")]
        public int UtenteGrupo7Id { get; set; }

        [ForeignKey("UtenteGrupo7Id")]
        public UtenteGrupo7? UtenteGrupo7 { get; set; }
        public ICollection<PlanoExercicioExercicio>? PlanoExercicioExercicios { get; set; }
    }
}
