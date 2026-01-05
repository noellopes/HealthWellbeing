namespace HealthWellbeing.Models
{
    public class TipoServicoViewModel
    {
        // A lista de entidades reais vinda do banco de dados
        public IEnumerable<TipoServicos> ListaServicos { get; set; }

        public Paginacao paginacao { get; set; }

        public string PesquisarNome { get; set; }
    }
}