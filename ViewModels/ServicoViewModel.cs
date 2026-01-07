using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModel
{
    public class ServicoViewModel
    {
        public IEnumerable<Servico> ListaServicos { get; set; }
        public Paginacao paginacao { get; set; } 
        public string PesquisarNome { get; set; }
    }
}
