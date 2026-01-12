using System;
using System.Collections.Generic;
using HealthWellbeing.Models;

namespace HealthWellbeing.Services
{
    public enum AjusteMotivoTipo
    {
        Alergia,
        Restricao,
        Meta
    }

    public enum AjusteIngredienteTipo
    {
        Nenhum,
        Substituido,
        Removido,
        PorcaoAjustada
    }

    public sealed class AjusteNota
    {
        public AjusteMotivoTipo Tipo { get; init; }
        public string Mensagem { get; init; } = string.Empty;
    }

    public sealed class IngredienteAjustado
    {
        public int AlimentoOriginalId { get; init; }
        public string AlimentoOriginalNome { get; init; } = string.Empty;

        public int? AlimentoFinalId { get; init; }
        public string? AlimentoFinalNome { get; init; }

        public UnidadeMedidaEnum UnidadeMedida { get; init; }

        public int QuantidadeOriginal { get; init; }
        public int QuantidadeFinal { get; init; }

        public AjusteIngredienteTipo TipoAjuste { get; init; }

        public bool IncluidoNoResultado => TipoAjuste != AjusteIngredienteTipo.Removido;
    }

    public sealed class ReceitaAjustada
    {
        public int ReceitaId { get; init; }
        public string Nome { get; init; } = string.Empty;
        public string? Descricao { get; init; }
        public int TempoPreparo { get; init; }

        public int PorcoesOriginal { get; init; }

        public decimal CaloriasPorPorcaoOriginal { get; init; }
        public decimal ProteinasPorPorcaoOriginal { get; init; }
        public decimal HidratosPorPorcaoOriginal { get; init; }
        public decimal GordurasPorPorcaoOriginal { get; init; }

        public decimal MultiplicadorPorcao { get; init; } = 1m;

        public decimal CaloriasPorPorcaoFinal { get; init; }
        public decimal ProteinasPorPorcaoFinal { get; init; }
        public decimal HidratosPorPorcaoFinal { get; init; }
        public decimal GordurasPorPorcaoFinal { get; init; }

        public IReadOnlyList<IngredienteAjustado> Ingredientes { get; init; } = Array.Empty<IngredienteAjustado>();
        public IReadOnlyList<AjusteNota> Notas { get; init; } = Array.Empty<AjusteNota>();

        public bool TemAjustes => Notas.Count > 0 || Math.Abs(MultiplicadorPorcao - 1m) > 0.001m;
    }

    public sealed class PlanoReceitasAjustadas
    {
        public string ClientId { get; init; } = string.Empty;
        public int PlanoAlimentarId { get; init; }

        public int? MetaId { get; init; }
        public string? MetaDescricao { get; init; }
        public int? MetaCaloriasDiarias { get; init; }

        public decimal MultiplicadorPorcaoGlobal { get; init; } = 1m;
        public IReadOnlyList<ReceitaAjustada> Receitas { get; init; } = Array.Empty<ReceitaAjustada>();

        public string? Aviso { get; init; }
    }
}
