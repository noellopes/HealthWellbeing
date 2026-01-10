using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class AssociarReceitasPlanoViewModel
    {
        public string? ClientId { get; set; }
        public string? ClientDisplay { get; set; }
        public int? PlanoAlimentarId { get; set; }

        public List<ClientPlanoOption> Clientes { get; set; } = new();
        public List<ReceitaOption> Receitas { get; set; } = new();

        public List<int> ReceitaIdsSelecionadas { get; set; } = new();

        public string? Aviso { get; set; }
    }

    public class ClientPlanoOption
    {
        public string ClientId { get; set; } = string.Empty;
        public int PlanoAlimentarId { get; set; }
        public string Display { get; set; } = string.Empty;
    }

    public class ReceitaOption
    {
        public int ReceitaId { get; set; }
        public string Display { get; set; } = string.Empty;
    }

    public class AssociarReceitasPlanoPostModel
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        public List<int> ReceitaIdsSelecionadas { get; set; } = new();
    }

    public class MinhasReceitasPlanoViewModel
    {
        public string ClientNome { get; set; } = string.Empty;
        public int PlanoAlimentarId { get; set; }

        public List<ReceitaResumo> Receitas { get; set; } = new();

        // Ajustes automáticos (apenas para apresentação na View de Receitas)
        public List<ReceitaAjustadaVm> ReceitasAjustadas { get; set; } = new();
        public string? Aviso { get; set; }
    }

    public class ReceitaAjustadaVm
    {
        public int ReceitaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int TempoPreparo { get; set; }

        public int PorcoesOriginal { get; set; }
        public decimal MultiplicadorPorcao { get; set; } = 1m;

        public decimal CaloriasPorPorcaoOriginal { get; set; }
        public decimal ProteinasPorPorcaoOriginal { get; set; }
        public decimal HidratosPorPorcaoOriginal { get; set; }
        public decimal GordurasPorPorcaoOriginal { get; set; }

        public decimal CaloriasPorPorcaoFinal { get; set; }
        public decimal ProteinasPorPorcaoFinal { get; set; }
        public decimal HidratosPorPorcaoFinal { get; set; }
        public decimal GordurasPorPorcaoFinal { get; set; }

        public List<IngredienteAjustadoVm> Ingredientes { get; set; } = new();
        public List<AjusteNotaVm> Notas { get; set; } = new();

        public bool TemAjustes => Notas.Count > 0 || System.Math.Abs(MultiplicadorPorcao - 1m) > 0.001m;
    }

    public class IngredienteAjustadoVm
    {
        public string AlimentoOriginalNome { get; set; } = string.Empty;
        public string? AlimentoFinalNome { get; set; }
        public string UnidadeMedida { get; set; } = string.Empty;
        public int QuantidadeOriginal { get; set; }
        public int QuantidadeFinal { get; set; }
        public string TipoAjuste { get; set; } = string.Empty;
        public bool IncluidoNoResultado { get; set; }
    }

    public class AjusteNotaVm
    {
        public string Tipo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
    }

    public class ReceitaResumo
    {
        public int ReceitaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int TempoPreparo { get; set; }
        public int Porcoes { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal HidratosCarbono { get; set; }
        public decimal Gorduras { get; set; }
    }
}
