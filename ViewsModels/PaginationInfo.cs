using System;
using System.Collections.Generic;

namespace HealthWellbeing.Models
{
    // Usamos <T> para tornar esta classe genérica (serve para Eventos, Livros, etc.)
    public class PaginationInfo<T>
    {
        // A lista de itens da página atual
        public IEnumerable<T> Items { get; set; }

        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }

        // Propriedades calculadas
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        // Lógica para mostrar os links das páginas (ex: 1 2 [3] 4 5)
        public int FirstPageShow { get; set; }
        public int LastPageShow { get; set; }
        private const int DisplayPages = 5; // Quantos números de página mostrar

        public PaginationInfo(IEnumerable<T> items, int totalItems, int pageSize, int currentPage)
        {
            Items = items;
            TotalItems = totalItems;
            PageSize = pageSize;
            CurrentPage = currentPage;

            // Calcular as páginas a mostrar
            FirstPageShow = CurrentPage - (DisplayPages / 2);
            if (FirstPageShow < 1)
            {
                FirstPageShow = 1;
            }

            LastPageShow = FirstPageShow + DisplayPages - 1;
            if (LastPageShow > TotalPages)
            {
                LastPageShow = TotalPages;
                // Recalcula o FirstPageShow se o LastPageShow foi ajustado
                FirstPageShow = Math.Max(1, TotalPages - DisplayPages + 1);
            }
        }
    }
}