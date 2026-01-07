using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModel
{
    public class TipoServicoViewModel
    {
        public IEnumerable<TipoServicos> ListaServicos { get; set; }

        public Paginacao paginacao { get; set; }

        public string PesquisarNome { get; set; }
    }
}
