using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.ViewModels
{
    // A restrição 'where T : class' garante que T seja um tipo de referência.
    public class PaginationInfo<T> where T : class
    {
        // Constante que define quantos links de página aparecerão antes e depois da página atual
        public const int NUMBER_PAGES_SHOW_BEFORE_AFTER = 5;

        // Propriedades do estado da paginação
        public IEnumerable<T>? Items { get; set; } = null;
        public int TotalItems { get; private set; }
        public int ItemsPerPage { get; private set; }
        public int CurrentPage { get; private set; }

        

        // Calcula o número total de páginas (arredondando para cima, mínimo 1)
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalItems / ItemsPerPage));

        // Define quantos itens o Entity Framework deve ignorar (Skip)
        public int ItemsToSkip => ItemsPerPage * (CurrentPage - 1);

        // Define o primeiro número de página a ser exibido nos links de navegação
        public int FirstPageShow => Math.Max(1, CurrentPage - NUMBER_PAGES_SHOW_BEFORE_AFTER);

        // Define o último número de página a ser exibido nos links de navegação
        public int LastPageShow => Math.Min(TotalPages, CurrentPage + NUMBER_PAGES_SHOW_BEFORE_AFTER);


        // Construtor
        public PaginationInfo(int currentPage, int totalItems, int itemsPerPage = 5) // ItemPerPages definido como 5 para testes
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            // Garante que a página atual não é menor que 1 nem maior que o TotalPages
            CurrentPage = Math.Clamp(currentPage, 1, TotalPages);
        }
    }
}