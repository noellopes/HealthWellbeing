namespace HealthWellbeing.ViewModels
{
    public class ExameMaterialViewModel
    {
        public int MaterialId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int StockAtual { get; set; }

        // Campos de input
        public bool Selecionado { get; set; }
        public int Quantidade { get; set; } = 1;
    }
}