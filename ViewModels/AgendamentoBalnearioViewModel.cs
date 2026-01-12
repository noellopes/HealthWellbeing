using HealthWellbeing.Models;
using System.Collections.Generic;

namespace HealthWellbeing.ViewModel
{
    public class AgendamentoBalnearioViewModel
    {
        public IEnumerable<AgendamentoBalneario> ListaAgendamentos { get; set; }
        public Paginacao paginacao { get; set; }
        public string PesquisarNomeUtente { get; set; }
        
    }
}
