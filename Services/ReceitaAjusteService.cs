using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Services
{
    public sealed class ReceitaAjusteService : IReceitaAjusteService
    {
        private readonly HealthWellbeingDbContext _context;

        public ReceitaAjusteService(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<PlanoReceitasAjustadas> GerarAjustesAsync(string clientId, int planoAlimentarId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return new PlanoReceitasAjustadas
                {
                    ClientId = string.Empty,
                    PlanoAlimentarId = planoAlimentarId,
                    Aviso = "Não foi possível identificar o cliente para ajustar as receitas."
                };
            }

            var plano = await _context.PlanoAlimentar
                .AsNoTracking()
                .Include(p => p.Meta)
                .FirstOrDefaultAsync(p => p.PlanoAlimentarId == planoAlimentarId && p.ClientId == clientId);

            if (plano == null)
            {
                return new PlanoReceitasAjustadas
                {
                    ClientId = clientId,
                    PlanoAlimentarId = planoAlimentarId,
                    Aviso = "Não foi encontrado um plano alimentar para este cliente."
                };
            }

            var receitaIds = await _context.ReceitasParaPlanosAlimentares
                .AsNoTracking()
                .Where(x => x.PlanoAlimentarId == planoAlimentarId)
                .Select(x => x.ReceitaId)
                .Distinct()
                .ToListAsync();

            if (receitaIds.Count == 0)
            {
                return new PlanoReceitasAjustadas
                {
                    ClientId = clientId,
                    PlanoAlimentarId = planoAlimentarId,
                    MetaId = plano.MetaId,
                    MetaDescricao = plano.Meta?.MetaDescription,
                    MetaCaloriasDiarias = plano.Meta?.DailyCalories,
                    Aviso = "Ainda não existem receitas associadas ao seu plano alimentar."
                };
            }

            var receitas = await _context.Receita
                .AsNoTracking()
                .Where(r => receitaIds.Contains(r.ReceitaId))
                .Include(r => r.Componentes)
                    .ThenInclude(c => c.Alimento)
                .ToListAsync();

            // --- Perfil alimentar do cliente (alergias + restrições) ---
            var alergiasCliente = await _context.ClientAlergia
                .AsNoTracking()
                .Where(x => x.ClientId == clientId)
                .Include(x => x.Alergia)
                .Select(x => x.Alergia!)
                .ToListAsync();

            var restricoesCliente = await _context.ClientRestricao
                .AsNoTracking()
                .Where(x => x.ClientId == clientId)
                .Include(x => x.RestricaoAlimentar)
                .Select(x => x.RestricaoAlimentar!)
                .ToListAsync();

            var alergiaIds = alergiasCliente.Select(a => a.AlergiaId).Distinct().ToList();
            var restricaoIds = restricoesCliente.Select(r => r.RestricaoAlimentarId).Distinct().ToList();

            var alergiasPorAlimento = new Dictionary<int, List<string>>();
            if (alergiaIds.Count > 0)
            {
                var pairs = await _context.AlergiaAlimento
                    .AsNoTracking()
                    .Where(x => alergiaIds.Contains(x.AlergiaId))
                    .ToListAsync();

                var alergiasById = alergiasCliente
                    .GroupBy(a => a.AlergiaId)
                    .ToDictionary(g => g.Key, g => g.First().Nome);

                foreach (var p in pairs)
                {
                    if (!alergiasById.TryGetValue(p.AlergiaId, out var nomeAlergia))
                        continue;

                    if (!alergiasPorAlimento.TryGetValue(p.AlimentoId, out var list))
                    {
                        list = new List<string>();
                        alergiasPorAlimento[p.AlimentoId] = list;
                    }

                    if (!list.Contains(nomeAlergia, StringComparer.OrdinalIgnoreCase))
                        list.Add(nomeAlergia);
                }
            }

            var restricoesPorAlimento = new Dictionary<int, List<string>>();
            if (restricaoIds.Count > 0)
            {
                var pairs = await _context.RestricaoAlimentarAlimento
                    .AsNoTracking()
                    .Where(x => restricaoIds.Contains(x.RestricaoAlimentarId))
                    .ToListAsync();

                var restricoesById = restricoesCliente
                    .GroupBy(r => r.RestricaoAlimentarId)
                    .ToDictionary(g => g.Key, g => g.First().Nome);

                foreach (var p in pairs)
                {
                    if (!restricoesById.TryGetValue(p.RestricaoAlimentarId, out var nomeRestricao))
                        continue;

                    if (!restricoesPorAlimento.TryGetValue(p.AlimentoId, out var list))
                    {
                        list = new List<string>();
                        restricoesPorAlimento[p.AlimentoId] = list;
                    }

                    if (!list.Contains(nomeRestricao, StringComparer.OrdinalIgnoreCase))
                        list.Add(nomeRestricao);
                }
            }

            bool IsConflitante(int alimentoId) => alergiasPorAlimento.ContainsKey(alimentoId) || restricoesPorAlimento.ContainsKey(alimentoId);

            // --- Multiplicador de porções baseado na meta (calorias) ---
            decimal multiplicadorPorcao = 1m;
            var meta = plano.Meta;

            if (meta != null)
            {
                var fatores = new List<decimal>();

                if (meta.DailyCalories > 0)
                {
                    var totalCaloriasBase = receitas.Sum(r => r.Calorias);
                    if (totalCaloriasBase > 0)
                        fatores.Add((decimal)meta.DailyCalories / totalCaloriasBase);
                }

                if (meta.DailyProtein > 0)
                {
                    var totalProteinaBase = receitas.Sum(r => r.Proteinas);
                    if (totalProteinaBase > 0)
                        fatores.Add((decimal)meta.DailyProtein / totalProteinaBase);
                }

                if (fatores.Count > 0)
                {
                    multiplicadorPorcao = fatores.Average();
                    multiplicadorPorcao = Clamp(multiplicadorPorcao, 0.75m, 1.25m);
                }
            }

            // --- Substituições pré-carregadas (apenas para ingredientes conflitantes no plano) ---
            var alimentosNoPlano = receitas
                .SelectMany(r => r.Componentes)
                .Select(c => c.AlimentoId)
                .Distinct()
                .ToList();

            var alimentosConflitantesNoPlano = alimentosNoPlano
                .Where(IsConflitante)
                .ToList();

            var possiveisSubstitutos = alimentosConflitantesNoPlano.Count == 0
                ? new List<AlimentoSubstituto>()
                : await _context.AlimentoSubstitutos
                    .AsNoTracking()
                    .Where(s => alimentosConflitantesNoPlano.Contains(s.AlimentoOriginalId))
                    .Where(s => s.AlimentoOriginalId != s.AlimentoSubstitutoRefId)
                    .Include(s => s.AlimentoSubstitutoRef)
                    .ToListAsync();

            var substitutosPorOriginal = possiveisSubstitutos
                .GroupBy(s => s.AlimentoOriginalId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var receitasAjustadas = new List<ReceitaAjustada>();

            foreach (var receita in receitas.OrderBy(r => r.Nome))
            {
                var notas = new List<AjusteNota>();
                var ingredientes = new List<IngredienteAjustado>();
                var substitutosJaUsadosNaReceita = new HashSet<int>();

                if (Math.Abs(multiplicadorPorcao - 1m) > 0.001m)
                {
                    var metaTxt = !string.IsNullOrWhiteSpace(meta?.MetaDescription)
                        ? $" para atender à meta: {meta!.MetaDescription}"
                        : " para alinhar com a sua meta diária";

                }

                foreach (var comp in receita.Componentes.OrderBy(c => c.ComponenteReceitaId))
                {
                    var alimento = comp.Alimento;
                    var alimentoNome = alimento?.Name ?? $"Alimento #{comp.AlimentoId}";

                    var motivos = new List<string>();
                    if (alergiasPorAlimento.TryGetValue(comp.AlimentoId, out var nomesAlergia) && nomesAlergia.Count > 0)
                    {
                        motivos.Add($"alergia ({string.Join(", ", nomesAlergia)})");
                    }
                    if (restricoesPorAlimento.TryGetValue(comp.AlimentoId, out var nomesRestricao) && nomesRestricao.Count > 0)
                    {
                        motivos.Add($"restrição ({string.Join(", ", nomesRestricao)})");
                    }

                    var temConflito = motivos.Count > 0;

                    if (!temConflito)
                    {
                        ingredientes.Add(new IngredienteAjustado
                        {
                            AlimentoOriginalId = comp.AlimentoId,
                            AlimentoOriginalNome = alimentoNome,
                            AlimentoFinalId = comp.AlimentoId,
                            AlimentoFinalNome = alimentoNome,
                            UnidadeMedida = comp.UnidadeMedida,
                            QuantidadeOriginal = comp.Quantidade,
                            QuantidadeFinal = AjustarQuantidade(comp.Quantidade, multiplicadorPorcao),
                            TipoAjuste = Math.Abs(multiplicadorPorcao - 1m) > 0.001m ? AjusteIngredienteTipo.PorcaoAjustada : AjusteIngredienteTipo.Nenhum
                        });
                        continue;
                    }

                    // Tenta substituir
                    var escolhido = EscolherMelhorSubstituto(
                        substitutosPorOriginal,
                        comp.AlimentoId,
                        IsConflitante,
                        substitutosJaUsadosNaReceita);

                    if (escolhido != null)
                    {
                        var subsId = escolhido.AlimentoSubstitutoRefId;
                        var subsNome = escolhido.AlimentoSubstitutoRef?.Name ?? $"Alimento #{subsId}";

                        substitutosJaUsadosNaReceita.Add(subsId);

                        var proporcao = escolhido.ProporcaoEquivalente ?? 1m;
                        var quantidadeFinal = AjustarQuantidade((decimal)comp.Quantidade * proporcao, multiplicadorPorcao);

                        ingredientes.Add(new IngredienteAjustado
                        {
                            AlimentoOriginalId = comp.AlimentoId,
                            AlimentoOriginalNome = alimentoNome,
                            AlimentoFinalId = subsId,
                            AlimentoFinalNome = subsNome,
                            UnidadeMedida = comp.UnidadeMedida,
                            QuantidadeOriginal = comp.Quantidade,
                            QuantidadeFinal = quantidadeFinal,
                            TipoAjuste = AjusteIngredienteTipo.Substituido
                        });

                        var motivoTxt = string.Join(" e ", motivos);
                        notas.Add(new AjusteNota
                        {
                            Tipo = motivos.Any(m => m.StartsWith("alergia", StringComparison.OrdinalIgnoreCase)) ? AjusteMotivoTipo.Alergia : AjusteMotivoTipo.Restricao,
                            Mensagem = $"Ingrediente substituído: {alimentoNome} → {subsNome} (devido a {motivoTxt})."
                        });

                        continue;
                    }

                    // Sem substituto: remove e informa
                    ingredientes.Add(new IngredienteAjustado
                    {
                        AlimentoOriginalId = comp.AlimentoId,
                        AlimentoOriginalNome = alimentoNome,
                        AlimentoFinalId = null,
                        AlimentoFinalNome = null,
                        UnidadeMedida = comp.UnidadeMedida,
                        QuantidadeOriginal = comp.Quantidade,
                        QuantidadeFinal = 0,
                        TipoAjuste = AjusteIngredienteTipo.Removido
                    });

                    notas.Add(new AjusteNota
                    {
                        Tipo = motivos.Any(m => m.StartsWith("alergia", StringComparison.OrdinalIgnoreCase)) ? AjusteMotivoTipo.Alergia : AjusteMotivoTipo.Restricao,
                        Mensagem = $"Ingrediente removido: {alimentoNome} (devido a {string.Join(" e ", motivos)}). Não existe substituto definido."
                    });
                }

                var receitaAjustada = new ReceitaAjustada
                {
                    ReceitaId = receita.ReceitaId,
                    Nome = receita.Nome,
                    Descricao = receita.Descricao,
                    TempoPreparo = receita.TempoPreparo,
                    PorcoesOriginal = receita.Porcoes,
                    CaloriasPorPorcaoOriginal = receita.Calorias,
                    ProteinasPorPorcaoOriginal = receita.Proteinas,
                    HidratosPorPorcaoOriginal = receita.HidratosCarbono,
                    GordurasPorPorcaoOriginal = receita.Gorduras,
                    MultiplicadorPorcao = multiplicadorPorcao,
                    CaloriasPorPorcaoFinal = Arredondar1(receita.Calorias * multiplicadorPorcao),
                    ProteinasPorPorcaoFinal = Arredondar1(receita.Proteinas * multiplicadorPorcao),
                    HidratosPorPorcaoFinal = Arredondar1(receita.HidratosCarbono * multiplicadorPorcao),
                    GordurasPorPorcaoFinal = Arredondar1(receita.Gorduras * multiplicadorPorcao),
                    Ingredientes = ingredientes,
                    Notas = notas
                };

                receitasAjustadas.Add(receitaAjustada);
            }

            return new PlanoReceitasAjustadas
            {
                ClientId = clientId,
                PlanoAlimentarId = planoAlimentarId,
                MetaId = meta?.MetaId,
                MetaDescricao = meta?.MetaDescription,
                MetaCaloriasDiarias = meta?.DailyCalories,
                MultiplicadorPorcaoGlobal = multiplicadorPorcao,
                Receitas = receitasAjustadas
            };
        }

        private static AlimentoSubstituto? EscolherMelhorSubstituto(
            Dictionary<int, List<AlimentoSubstituto>> substitutosPorOriginal,
            int alimentoOriginalId,
            Func<int, bool> isConflitante,
            IReadOnlySet<int> substitutosJaUsadosNaReceita)
        {
            if (!substitutosPorOriginal.TryGetValue(alimentoOriginalId, out var lista) || lista.Count == 0)
                return null;

            return lista
                .Where(s => s.AlimentoSubstitutoRefId != alimentoOriginalId)
                .Where(s => !isConflitante(s.AlimentoSubstitutoRefId))
                .Where(s => !substitutosJaUsadosNaReceita.Contains(s.AlimentoSubstitutoRefId))
                .OrderByDescending(s => s.FatorSimilaridade ?? 0d)
                .ThenByDescending(s => s.ProporcaoEquivalente ?? 1m)
                .FirstOrDefault();
        }

        private static int AjustarQuantidade(int quantidade, decimal multiplicador) => AjustarQuantidade((decimal)quantidade, multiplicador);

        private static int AjustarQuantidade(decimal quantidade, decimal multiplicador)
        {
            var valor = quantidade * multiplicador;
            var arred = (int)Math.Round(valor, MidpointRounding.AwayFromZero);
            return Math.Max(1, arred);
        }

        private static decimal Arredondar1(decimal valor) => Math.Round(valor, 1, MidpointRounding.AwayFromZero);

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
