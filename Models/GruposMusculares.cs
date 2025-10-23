using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class GruposMusculares
    {
        [Required(ErrorMessage = "O ID do músculo é obrigatório.")]
        public int MusculoId { get; set; } // ID

        [Required(ErrorMessage = "O nome do músculo é obrigatório.")]
        public string MusculoNome { get; set; } // Nome do músculo

        [Required(ErrorMessage = "O grupo muscular primário é obrigatório.")]
        public string GrupoMuscularPrimario { get; set; } // Ex: Peitoral, Dorsal, Quadríceps...

        [Required(ErrorMessage = "O lado do músculo é obrigatório.")]
        public string LadoMusculo { get; set; } // Ex: Esquerdo ou Direito

        [Required(ErrorMessage = "O tamanho do músculo é obrigatório.")]
        public double TamanhoMusculo { get; set; } = 0.0; // Tamanho do músculo (em cm ou cm²)
    }
}
