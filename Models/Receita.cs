using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Receita
    {
        [Key]
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

        [Required(ErrorMessage = "O tempo de preparo é obrigatório.")]
        [Range(1, 1440, ErrorMessage = "O tempo de preparo deve estar entre 1 e 1440 minutos.")]
        [Display(Name = "Tempo de Preparo (minutos)")]
        public int TempoPreparo { get; set; }

        [Required(ErrorMessage = "O número de porções é obrigatório.")]
        [Range(1, 100, ErrorMessage = "O número de porções deve estar entre 1 e 100.")]
        [Display(Name = "Número de Porções")]
        public int Porcoes { get; set; }

        [Required(ErrorMessage = "As calorias por porção são obrigatórias.")]
        [Range(0, 10000, ErrorMessage = "As calorias por porção devem estar entre 0 e 10000.")]
        [Display(Name = "Calorias por Porção")]
        public decimal CaloriasPorPorcao { get; set; }

        [Required(ErrorMessage = "As proteínas são obrigatórias.")]
        [Range(0, 1000, ErrorMessage = "As proteínas devem estar entre 0 e 1000g.")]
        [Display(Name = "Proteínas (g)")]
        public decimal Proteinas { get; set; }

        [Required(ErrorMessage = "Os hidratos de carbono são obrigatórios.")]
        [Range(0, 1000, ErrorMessage = "Os hidratos de carbono devem estar entre 0 e 1000g.")]
        [Display(Name = "Hidratos de Carbono (g)")]
        public decimal HidratosCarbono { get; set; }

        [Required(ErrorMessage = "As gorduras são obrigatórias.")]
        [Range(0, 1000, ErrorMessage = "As gorduras devem estar entre 0 e 1000g.")]
        [Display(Name = "Gorduras (g)")]
        public decimal Gorduras { get; set; }

        [Display(Name = "Vegetariana")]
        public bool IsVegetariana { get; set; }

        [Display(Name = "Vegana")]
        public bool IsVegan { get; set; }

        [Display(Name = "Sem Lactose")]
        public bool IsLactoseFree { get; set; }

        //public ICollection<ComponenteReceita> Componentes { get; set; } --> Quando implementar componentes da receita

        //public ICollection<Alergia> RestricaoAlergias { get; set; } --> Quando implementar restrições alimentares

        //public ICollection<CategoriaAlimento> CategoriasAlimentos { get; set; } --> Quando implementar categorias

    }
}