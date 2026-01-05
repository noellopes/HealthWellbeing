using System.Collections.Generic;

namespace HealthWellbeing.ViewModels
{
    public class UtenteEstatisticasViewModel
    {
        public string NomeUtente { get; set; }

        // --- Estatísticas Gerais  ---
        public string ExercicioFavorito { get; set; } = "Nenhum";
        public double VolumeTotalAcumulado { get; set; } 
        public int TotalPlanosAtribuidos { get; set; }
        public int TotalPlanos100Porcento { get; set; } 

        // --- Dados para o Gráfico ---
        public int QtdConcluidos { get; set; }
        public int QtdNaoConcluidos { get; set; }
        public int TotalExercicios { get; set; }
        public int PercentagemConclusao => TotalExercicios == 0 ? 0 : (int)((double)QtdConcluidos / TotalExercicios * 100);

        // --- Dados para o Gráfico de Barras ---
        public List<DadoBarra> EvolucaoCarga { get; set; } = new List<DadoBarra>();
    }

    public class DadoBarra
    {
        public string Label { get; set; }
        public double ValorReal { get; set; }
        public int AlturaPercentagem { get; set; }
    }
}