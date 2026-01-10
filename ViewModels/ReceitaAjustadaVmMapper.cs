using System.Linq;
using HealthWellbeing.Services;

namespace HealthWellbeing.ViewModels
{
    public static class ReceitaAjustadaVmMapper
    {
        public static ReceitaAjustadaVm ToVm(this ReceitaAjustada receita)
        {
            return new ReceitaAjustadaVm
            {
                ReceitaId = receita.ReceitaId,
                Nome = receita.Nome,
                Descricao = receita.Descricao,
                TempoPreparo = receita.TempoPreparo,
                PorcoesOriginal = receita.PorcoesOriginal,
                MultiplicadorPorcao = receita.MultiplicadorPorcao,
                CaloriasPorPorcaoOriginal = receita.CaloriasPorPorcaoOriginal,
                ProteinasPorPorcaoOriginal = receita.ProteinasPorPorcaoOriginal,
                HidratosPorPorcaoOriginal = receita.HidratosPorPorcaoOriginal,
                GordurasPorPorcaoOriginal = receita.GordurasPorPorcaoOriginal,
                CaloriasPorPorcaoFinal = receita.CaloriasPorPorcaoFinal,
                ProteinasPorPorcaoFinal = receita.ProteinasPorPorcaoFinal,
                HidratosPorPorcaoFinal = receita.HidratosPorPorcaoFinal,
                GordurasPorPorcaoFinal = receita.GordurasPorPorcaoFinal,
                Ingredientes = receita.Ingredientes
                    .Select(i => new IngredienteAjustadoVm
                    {
                        AlimentoOriginalNome = i.AlimentoOriginalNome,
                        AlimentoFinalNome = i.AlimentoFinalNome,
                        UnidadeMedida = i.UnidadeMedida.ToString(),
                        QuantidadeOriginal = i.QuantidadeOriginal,
                        QuantidadeFinal = i.QuantidadeFinal,
                        TipoAjuste = i.TipoAjuste.ToString(),
                        IncluidoNoResultado = i.IncluidoNoResultado
                    })
                    .ToList(),
                Notas = receita.Notas
                    .Select(n => new AjusteNotaVm
                    {
                        Tipo = n.Tipo.ToString(),
                        Mensagem = n.Mensagem
                    })
                    .ToList()
            };
        }
    }
}
