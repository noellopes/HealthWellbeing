using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public enum TipoRestricao
    {
        Vegetariana,
        Vegana,
        IntoleranciaLactose,
        IntoleranciaGluten,
        SemAcucar,
        BaixoSodio,
        Religiosa,
        Outra
    }

    public enum GravidadeRestricao
    {
        Leve,
        Moderada,
        Grave
    }

    public class RestricaoAlimentar
    {
        [Key]
        public int RestricaoAlimentarId { get; set; }

        [Required(ErrorMessage = "O nome da restrição é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome da Restrição")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O tipo da restrição é obrigatório.")]
        [Display(Name = "Tipo de Restrição")]
        public TipoRestricao Tipo { get; set; }

        [Required(ErrorMessage = "A gravidade da restrição é obrigatória.")]
        [Display(Name = "Gravidade")]
        public GravidadeRestricao Gravidade { get; set; }

        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        // === NOVA RELAÇÃO N:N COM ALIMENTOS ===
        public ICollection<RestricaoAlimentarAlimento>? AlimentosAssociados { get; set; }

        // Relação opcional com receitas (continua igual)
        [Display(Name = "Receitas Afetadas")]
        public ICollection<Receita>? Receitas { get; set; }

        // Relação opcional com substitutos
        public ICollection<AlimentoSubstituto>? Substitutos { get; set; }
        public ICollection<ClientRestricao>? ClientesAssociados { get; set; }
    }
}
