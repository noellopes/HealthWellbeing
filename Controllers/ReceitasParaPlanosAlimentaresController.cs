using System.Diagnostics;
using HealthWellbeing.Models;
using HealthWellbeing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize] // Restringe o acesso a usuários autenticados
    public class ReceitasParaPlanosAlimentaresController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ReceitasParaPlanosAlimentaresController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult ReceitasParaPlanosAlimentares()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                ViewData["AccessDenied"] = true;
                return View();
            }

            return View();
        }

        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Associar(string? clientId)
        {
            var vm = new AssociarReceitasPlanoViewModel();

            vm.Clientes = await _context.PlanoAlimentar
                .AsNoTracking()
                .Include(p => p.Client)
                .Where(p => p.Client != null)
                .OrderBy(p => p.Client!.Name)
                .Select(p => new ClientPlanoOption
                {
                    ClientId = p.ClientId,
                    PlanoAlimentarId = p.PlanoAlimentarId,
                    Display = p.Client!.Email != null && p.Client.Email != ""
                        ? $"{p.Client.Name} ({p.Client.Email})"
                        : p.Client.Name
                })
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(clientId))
            {
                vm.Aviso = "Selecione um cliente para gerir as receitas do seu plano alimentar.";
                return View(vm);
            }

            var plano = await _context.PlanoAlimentar
                .AsNoTracking()
                .Include(p => p.Client)
                .FirstOrDefaultAsync(p => p.ClientId == clientId);

            if (plano == null)
            {
                vm.Aviso = "Não foi encontrado um plano alimentar para o cliente selecionado.";
                return View(vm);
            }

            vm.ClientId = clientId;
            vm.PlanoAlimentarId = plano.PlanoAlimentarId;
            vm.ClientDisplay = plano.Client?.Name;

            vm.ReceitaIdsSelecionadas = await _context.ReceitasParaPlanosAlimentares
                .AsNoTracking()
                .Where(x => x.PlanoAlimentarId == plano.PlanoAlimentarId)
                .Select(x => x.ReceitaId)
                .ToListAsync();

            vm.Receitas = await _context.Receita
                .AsNoTracking()
                .OrderBy(r => r.Nome)
                .Select(r => new ReceitaOption
                {
                    ReceitaId = r.ReceitaId,
                    Display = $"{r.Nome} • {r.Calorias} kcal/porção • {r.Porcoes} porções • {r.TempoPreparo} min"
                })
                .ToListAsync();

            return View(vm);
        }

        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Associar(AssociarReceitasPlanoPostModel input)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Associar), new { clientId = input.ClientId });
            }

            var plano = await _context.PlanoAlimentar
                .FirstOrDefaultAsync(p => p.ClientId == input.ClientId);

            if (plano == null)
            {
                TempData["AlertMessage"] = "Não foi encontrado um plano alimentar para o cliente selecionado.";
                TempData["AlertType"] = "danger";
                return RedirectToAction(nameof(Associar), new { clientId = input.ClientId });
            }

            var selecionadas = (input.ReceitaIdsSelecionadas ?? new List<int>())
                .Distinct()
                .ToHashSet();

            var existentes = await _context.ReceitasParaPlanosAlimentares
                .Where(x => x.PlanoAlimentarId == plano.PlanoAlimentarId)
                .ToListAsync();

            var remover = existentes.Where(x => !selecionadas.Contains(x.ReceitaId)).ToList();
            if (remover.Count > 0)
                _context.ReceitasParaPlanosAlimentares.RemoveRange(remover);

            var existentesSet = existentes.Select(x => x.ReceitaId).ToHashSet();
            var adicionar = selecionadas
                .Where(id => !existentesSet.Contains(id))
                .Select(id => new ReceitasParaPlanosAlimentares
                {
                    PlanoAlimentarId = plano.PlanoAlimentarId,
                    ReceitaId = id
                })
                .ToList();

            if (adicionar.Count > 0)
                await _context.ReceitasParaPlanosAlimentares.AddRangeAsync(adicionar);

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Receitas do plano alimentar atualizadas com sucesso.";
            TempData["AlertType"] = "success";

            return RedirectToAction(nameof(Associar), new { clientId = input.ClientId });
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MinhasReceitas()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }

            var client = await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);

            if (client == null)
            {
                var emptyVm = new MinhasReceitasPlanoViewModel
                {
                    Aviso = "Não foi possível localizar o seu perfil de cliente. Contacte o suporte." 
                };
                return View(emptyVm);
            }

            var plano = await _context.PlanoAlimentar
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ClientId == client.ClientId);

            if (plano == null)
            {
                var noPlanVm = new MinhasReceitasPlanoViewModel
                {
                    ClientNome = client.Name,
                    Aviso = "Ainda não existe um plano alimentar associado ao seu perfil." 
                };
                return View(noPlanVm);
            }

            var receitas = await _context.ReceitasParaPlanosAlimentares
                .AsNoTracking()
                .Where(x => x.PlanoAlimentarId == plano.PlanoAlimentarId)
                .Include(x => x.Receita)
                .Select(x => x.Receita!)
                .OrderBy(r => r.Nome)
                .Select(r => new ReceitaResumo
                {
                    ReceitaId = r.ReceitaId,
                    Nome = r.Nome,
                    Descricao = r.Descricao,
                    TempoPreparo = r.TempoPreparo,
                    Porcoes = r.Porcoes,
                    Calorias = r.Calorias,
                    Proteinas = r.Proteinas,
                    HidratosCarbono = r.HidratosCarbono,
                    Gorduras = r.Gorduras
                })
                .ToListAsync();

            var vm = new MinhasReceitasPlanoViewModel
            {
                ClientNome = client.Name,
                PlanoAlimentarId = plano.PlanoAlimentarId,
                Receitas = receitas,
                Aviso = receitas.Count == 0 ? "Ainda não existem receitas associadas ao seu plano alimentar." : null
            };

            return View(vm);
        }
    }
}
