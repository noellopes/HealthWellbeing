namespace HealthWellbeing.Models
{
    
        public class Paginacao
        {
            public int PaginaCorrente { get; set; }
            public int ItemTotal { get; set; }
            public int TamanhoPagina { get; set; } = 6;
            public int PaginaTotal => (int)Math.Ceiling((decimal)ItemTotal / TamanhoPagina);
            public int PrimeiraPaginaVer => Math.Max(1, PaginaCorrente - 5);
            public int UltimaPaginaVer => Math.Min(PaginaTotal, PaginaCorrente + 5);
        }
    }
