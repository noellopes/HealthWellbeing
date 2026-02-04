using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Exercicio
    {
        public int ExercicioId { get; set; }

        [Required]
        [StringLength(100)]
        public string ExercicioNome { get; set; } = string.Empty;

        [Required]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 480)]
        public double Duracao { get; set; }

        [Required]
        [Range(1, 10)]
        public int Intencidade { get; set; }

        [Required]
        [Range(0.1, 2000)]
        public double CaloriasGastas { get; set; }

        [Required]
        public string Instrucoes { get; set; } = string.Empty;

        [Required]
        public string EquipamentoNecessario { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        public int Repeticoes { get; set; }

        [Required]
        [Range(1, 100)]
        public int Series { get; set; }

        public ICollection<Genero>? Genero { get; set; }
        public ICollection<GrupoMuscular>? GrupoMuscular { get; set; }
    }
}