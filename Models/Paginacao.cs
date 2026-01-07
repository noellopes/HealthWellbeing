namespace HealthWellbeing.Models
{
    
        public class Paginacao
        {
        private int totalRegistos;
        private int pagina;
        private int pageSize;

        public Paginacao(int totalRegistos, int pagina, int pageSize)
        {
            this.totalRegistos = totalRegistos;
            this.pagina = pagina;
            this.pageSize = pageSize;
        }

        public int PaginaCorrente { get; set; }
            public int ItemTotal { get; set; }
            public int TamanhoPagina { get; set; } = 6;
            public int PaginaTotal => (int)Math.Ceiling((decimal)ItemTotal / TamanhoPagina);
            public int PrimeiraPaginaVer => Math.Max(1, PaginaCorrente - 5);
            public int UltimaPaginaVer => Math.Min(PaginaTotal, PaginaCorrente + 5);
        }
    }
