namespace HealthWellbeing.Models
{
    public class GruposMusculares
    {
        public int MusculoId { get; set; } // ID

        public string MusculoNome { get; set; } // Nome do músculo

        public string GrupoMuscularPrimario { get; set; } // Ex: Peitoral, Dorsal, Quadríceps...

        public string LadoMusculo { get; set; } // Ex: Esquerdo ou Direito

        public double TamanhoMusculo { get; set; } = 0.0; // Tamanho do músculo (em cm ou cm²)
    }
}
