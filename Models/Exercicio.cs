using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exercicio
    {
        //e precisso colocar "[Required]"??
        public int ExercicioId { get; set; }


        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
        public string ExercicioNome { get; set; }


        [Required(ErrorMessage = "Descricao é obrigatória")]
        public string Descricao { get; set; }


        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(0.1, 480, ErrorMessage = "Duração deve ser entre 0.1 e 480 minutos")]
        public double Duracao { get; set; }
        //Duracao em minutos


        [Required(ErrorMessage = "Intensidade é obrigatória")]
        [Range(1, 10, ErrorMessage = "Intensidade deve ser entre 1 e 10")]
        public int Intencidade { get; set; }


        [Required(ErrorMessage = "Calorias Gastas é obrigatório")]
        [Range(0, 5000, ErrorMessage = "Calorias devem ser entre 0 e 5000")]
        public double CaloriasGastas { get; set; }


        [Required(ErrorMessage = "E necessario uma Instrução")]
        public string Instrucoes { get; set; }


        [Required(ErrorMessage = "Equipamento é obrigatório")]
        [StringLength(50)]
        public string EquipamentoNecessario { get; set; }


        [Required(ErrorMessage = "Repetições é obrigatório")]
        [Range(1, 1000)]
        public int Repeticoes { get; set; }


        [Required(ErrorMessage = "Series é obrigatório")]
        [Range(1, 50)]
        public int Series { get; set; }


        [Required(ErrorMessage = "Grupo Muscular é obrigatório")]
        public List<GruposMusculares>? GruposMusculares { get; set; }
        //Caixa de Selecao


        [Required(ErrorMessage = "Género é obrigatorio")]
        public string Genero { get; set; } // "Masculino", "Feminino", "Unissexo"
        //Caixa de Selecao

    }
}
