namespace HealthWellbeing.Models
{
    public class Paginacao
    {
        public int PaginaCorrente { get; private set; }
        public int ItemTotal { get; private set; }
        public int TamanhoPagina { get; private set; }

        public int PaginaTotal =>
            (int)Math.Ceiling((double)ItemTotal / TamanhoPagina);

        public int PrimeiraPaginaVer =>
            Math.Max(1, PaginaCorrente - 2);

        public int UltimaPaginaVer =>
            Math.Min(PaginaTotal, PaginaCorrente + 2);

        public Paginacao(int totalRegistos, int pagina, int pageSize)
        {
            ItemTotal = totalRegistos;
            PaginaCorrente = pagina < 1 ? 1 : pagina;
            TamanhoPagina = pageSize;
        }
    }
}
