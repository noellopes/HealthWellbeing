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
        public string? Aviso { get; set; }
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
