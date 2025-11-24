using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Receita
    {
        public int ReceitaId { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome da Receita")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O modo de preparo é obrigatório.")]
        [StringLength(2000, ErrorMessage = "O modo de preparo deve ter no máximo 2000 caracteres.")]
        [Display(Name = "Modo de Preparo")]
        public string ModoPreparo { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "O tempo de preparo deve estar entre 1 e 1440 minutos.")]
        [Display(Name = "Tempo de Preparo (minutos)")]
        public int TempoPreparo { get; set; }

        [Range(1, 100, ErrorMessage = "O número de porções deve estar entre 1 e 100.")]
        [Display(Name = "Porções")]
        public int Porcoes { get; set; }

        [Range(0, 10000)]
        [Display(Name = "Calorias por Porção")]
        public decimal Calorias { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Proteínas (g)")]
        public decimal Proteinas { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Carboidratos (g)")]
        public decimal HidratosCarbono { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Gorduras (g)")]
        public decimal Gorduras { get; set; }

        // N:N relationship with ComponenteReceita through ReceitaComponente
        public ICollection<ReceitaComponente> ReceitaComponentes { get; set; } = new List<ReceitaComponente>();
        public ICollection<ComponenteReceita> Componentes { get; set; } = new List<ComponenteReceita>();

        // N:N relationship with RestricaoAlimentar (keeping existing structure)
        public List<int> RestricoesAlimentarId { get; set; } = new List<int>();
        public ICollection<RestricaoAlimentar>? RestricoesAlimentares { get; set; }
    }
}
