using System.Collections.Generic;

namespace HealthWellbeing.ViewModels
{
    // Representa uma única checkbox (um material/equipamento)
    public class RecursoCheckBoxItem
    {
        public int Id { get; set; }
        public string Nome { get; set; } // Ex: "Máquina Raio-X"
        public bool IsSelected { get; set; } // Se está marcado ou não
    }

    // O ViewModel principal para a View de Edição
    public class TipoExameRecursosViewModel
    {
        public int ExameTipoId { get; set; }
        public string NomeExame { get; set; }
        public string Descricao { get; set; }
        public int EspecialidadeId { get; set; }

        // Lista de todas as checkboxes para exibir na View
        public List<RecursoCheckBoxItem> Recursos { get; set; } = new List<RecursoCheckBoxItem>();
    }
}