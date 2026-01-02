namespace HealthWellbeing.ViewModels
{
    public class TreinoChecklistViewModel
    {
        public int PlanoId { get; set; }
        public string NomePlano { get; set; }

        public List<ExercicioItemViewModel> Exercicios { get; set; } = new List<ExercicioItemViewModel>();
    }

    public class ExercicioItemViewModel
    {
        public int ExercicioId { get; set; }
        public string NomeExercicio { get; set; }
        public string Descricao { get; set; }
        public bool EstaConcluido { get; set; }
    }
}