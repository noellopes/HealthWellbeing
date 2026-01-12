using System;
using System.Collections.Generic;
using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public class ClientSearchViewModel
    {
        // Parâmetros de filtro
        public string SearchName { get; set; }
        public string SearchEmail { get; set; }
        public string SearchGender { get; set; }
        public DateTime? SearchBirthDateFrom { get; set; }
        public DateTime? SearchBirthDateTo { get; set; }
        public DateTime? SearchRegistrationDateFrom { get; set; }
        public DateTime? SearchRegistrationDateTo { get; set; }

        // Paginação
        public List<Client> Clients { get; set; } = new List<Client>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        // Informações úteis
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
