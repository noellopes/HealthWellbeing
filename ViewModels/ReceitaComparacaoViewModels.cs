using System.Collections.Generic;
using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public sealed class ReceitaComparacaoDetailsViewModel
    {
        public Receita Receita { get; init; } = null!;

        // Versão adaptada (gerada dinamicamente; nunca persistida)
        public ReceitaAjustadaVm? ReceitaAdaptada { get; init; }
        public string? AvisoAdaptacao { get; init; }

        // Contexto Nutricionista
        public string? ClientIdSelecionado { get; init; }
        public List<ClienteOptionVm> ClientesDisponiveis { get; init; } = new();

        public bool TemVersaoAdaptada => ReceitaAdaptada != null;
    }

    public sealed class ClienteOptionVm
    {
        public string ClientId { get; init; } = string.Empty;
        public int PlanoAlimentarId { get; init; }
        public string Display { get; init; } = string.Empty;
    }
}
