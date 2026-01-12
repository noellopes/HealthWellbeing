using HealthWellbeing.Models;
using System.Collections.Generic;

namespace HealthWellbeing.ViewModels
{
    public class UtentesDashboardViewModel
    {
        public int TotalUtentes { get; set; }
        public int UtentesAtivos { get; set; }
        public int UtentesInativos { get; set; }

        public int TotalTerapeutas { get; set; }
        public int TotalServicos { get; set; }
        public int TotalAgendamentos { get; set; }

        public List<UtenteBalneario> Utentes { get; set; } = new();
    }
}
