using System.Diagnostics;
using HealthWellbeing.Models;
using HealthWellbeing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HealthWellbeing.ViewModels;
using HealthWellbeing.Services;

namespace HealthWellbeing.Controllers
{
    [Authorize] // Restringe o acesso a usuários autenticados
    public class ReceitasParaPlanosAlimentaresController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly IReceitaAjusteService _ajusteService;

        public ReceitasParaPlanosAlimentaresController(HealthWellbeingDbContext context, IReceitaAjusteService ajusteService)
        {
            _context = context;
            _ajusteService = ajusteService;
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

            // Ajustes automáticos (apenas para apresentação; não persiste alterações)
            var ajustado = await _ajusteService.GerarAjustesAsync(client.ClientId, plano.PlanoAlimentarId);

            var vm = new MinhasReceitasPlanoViewModel
            {
                ClientNome = client.Name,
                PlanoAlimentarId = plano.PlanoAlimentarId,
                ReceitasAjustadas = ajustado.Receitas.Select(r => r.ToVm()).ToList(),
                Aviso = ajustado.Aviso
            };

            return View(vm);
        }

        // ---------------------------
        // Perfil Alimentar (Alergias, Restrições, Metas)
        // ---------------------------

        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> ClientesPerfilAlimentar()
        {
            var vm = new ClientesPerfilAlimentarViewModel
            {
                Clientes = await _context.Client
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new ClienteResumoVm
                    {
                        ClientId = c.ClientId,
                        Nome = c.Name,
                        Email = c.Email
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> GerirPerfilAlimentar(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return NotFound();

            var client = await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null)
                return NotFound();

            var vm = await BuildPerfilAlimentarVm(clientId, client.Name, client.Email);
            return View(vm);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MeuPerfilAlimentar()
        {
            var client = await GetCurrentClientAsync();
            if (client == null)
                return View(new MeuPerfilAlimentarViewModel { Aviso = "Não foi possível localizar o seu perfil de cliente." });

            var vmBase = await BuildPerfilAlimentarVm(client.ClientId, client.Name, client.Email);

            var vm = new MeuPerfilAlimentarViewModel
            {
                ClientNome = client.Name,
                Alergias = vmBase.Alergias,
                Restricoes = vmBase.Restricoes,
                Metas = vmBase.Metas,
                OpcoesAlergias = vmBase.OpcoesAlergias,
                OpcoesRestricoes = vmBase.OpcoesRestricoes,
                Aviso = vmBase.Aviso
            };

            return View(vm);
        }

        // ---- Alergias (ClientAlergia) ----

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarAlergia(AddAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            var exists = await _context.ClientAlergia
                .AnyAsync(x => x.ClientId == clientId && x.AlergiaId == input.ItemId);

            if (!exists)
            {
                _context.ClientAlergia.Add(new ClientAlergia { ClientId = clientId, AlergiaId = input.ItemId });
                await _context.SaveChangesAsync();
            }

            TempData["AlertMessage"] = "Alergia associada com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAlergia(EditAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            if (input.OldItemId == input.NewItemId)
                return RedirectAfterPerfilWrite(clientId);

            var current = await _context.ClientAlergia
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.AlergiaId == input.OldItemId);

            if (current != null)
                _context.ClientAlergia.Remove(current);

            var already = await _context.ClientAlergia
                .AnyAsync(x => x.ClientId == clientId && x.AlergiaId == input.NewItemId);

            if (!already)
                _context.ClientAlergia.Add(new ClientAlergia { ClientId = clientId, AlergiaId = input.NewItemId });

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Alergia atualizada com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverAlergia(DeleteAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            var current = await _context.ClientAlergia
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.AlergiaId == input.ItemId);

            if (current != null)
            {
                _context.ClientAlergia.Remove(current);
                await _context.SaveChangesAsync();
            }

            TempData["AlertMessage"] = "Alergia removida com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        // ---- Restrições (ClientRestricao) ----

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarRestricao(AddAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            var exists = await _context.ClientRestricao
                .AnyAsync(x => x.ClientId == clientId && x.RestricaoAlimentarId == input.ItemId);

            if (!exists)
            {
                _context.ClientRestricao.Add(new ClientRestricao { ClientId = clientId, RestricaoAlimentarId = input.ItemId });
                await _context.SaveChangesAsync();
            }

            TempData["AlertMessage"] = "Restrição alimentar associada com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarRestricao(EditAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            if (input.OldItemId == input.NewItemId)
                return RedirectAfterPerfilWrite(clientId);

            var current = await _context.ClientRestricao
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.RestricaoAlimentarId == input.OldItemId);

            if (current != null)
                _context.ClientRestricao.Remove(current);

            var already = await _context.ClientRestricao
                .AnyAsync(x => x.ClientId == clientId && x.RestricaoAlimentarId == input.NewItemId);

            if (!already)
                _context.ClientRestricao.Add(new ClientRestricao { ClientId = clientId, RestricaoAlimentarId = input.NewItemId });

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Restrição alimentar atualizada com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverRestricao(DeleteAssociacaoPost input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            var current = await _context.ClientRestricao
                .FirstOrDefaultAsync(x => x.ClientId == clientId && x.RestricaoAlimentarId == input.ItemId);

            if (current != null)
            {
                _context.ClientRestricao.Remove(current);
                await _context.SaveChangesAsync();
            }

            TempData["AlertMessage"] = "Restrição alimentar removida com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(clientId);
        }

        // ---- Metas (Meta) ----

        [Authorize(Roles = "Nutricionista,Cliente")]
        public async Task<IActionResult> CriarMeta(string? clientId, string? returnUrl)
        {
            var resolved = await ResolveClientIdForReadAsync(clientId);
            if (resolved == null)
                return Forbid();

            var vm = new MetaEditViewModel
            {
                ClientId = resolved,
                ReturnUrl = NormalizeReturnUrl(returnUrl) ?? NormalizeReturnUrlFromReferer()
            };
            return View(vm);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarMeta(MetaEditViewModel input)
        {
            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            if (!ModelState.IsValid)
                return View(input);

            _context.Meta.Add(new Meta
            {
                ClientId = clientId,
                MetaDescription = input.MetaDescription,
                DailyCalories = input.DailyCalories,
                DailyProtein = input.DailyProtein,
                DailyFat = input.DailyFat,
                DailyHydrates = input.DailyHydrates,
                DailyVitamins = input.DailyVitamins
            });

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Meta criada com sucesso.";
            TempData["AlertType"] = "success";

            if (User.IsInRole("Nutricionista"))
            {
                var safeReturn = NormalizeReturnUrl(input.ReturnUrl);
                if (!string.IsNullOrWhiteSpace(safeReturn))
                    return Redirect(safeReturn);
            }

            return RedirectAfterPerfilWrite(clientId);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        public async Task<IActionResult> EditarMeta(int id, string? returnUrl)
        {
            var meta = await _context.Meta
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MetaId == id);

            if (meta == null)
                return NotFound();

            var allowedClientId = await ResolveClientIdForReadAsync(meta.ClientId);
            if (allowedClientId == null)
                return Forbid();

            var vm = new MetaEditViewModel
            {
                MetaId = meta.MetaId,
                ClientId = meta.ClientId,
                ReturnUrl = NormalizeReturnUrl(returnUrl) ?? NormalizeReturnUrlFromReferer(),
                MetaDescription = meta.MetaDescription,
                DailyCalories = meta.DailyCalories,
                DailyProtein = meta.DailyProtein,
                DailyFat = meta.DailyFat,
                DailyHydrates = meta.DailyHydrates,
                DailyVitamins = meta.DailyVitamins
            };
            return View(vm);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarMeta(MetaEditViewModel input)
        {
            if (input.MetaId == null)
                return NotFound();

            var clientId = await ResolveClientIdForWriteAsync(input.ClientId);
            if (clientId == null)
                return Forbid();

            if (!ModelState.IsValid)
                return View(input);

            var meta = await _context.Meta
                .FirstOrDefaultAsync(m => m.MetaId == input.MetaId.Value);

            if (meta == null)
                return NotFound();

            if (!string.Equals(meta.ClientId, clientId, System.StringComparison.Ordinal))
                return Forbid();

            meta.MetaDescription = input.MetaDescription;
            meta.DailyCalories = input.DailyCalories;
            meta.DailyProtein = input.DailyProtein;
            meta.DailyFat = input.DailyFat;
            meta.DailyHydrates = input.DailyHydrates;
            meta.DailyVitamins = input.DailyVitamins;

            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Meta atualizada com sucesso.";
            TempData["AlertType"] = "success";

            if (User.IsInRole("Nutricionista"))
            {
                var safeReturn = NormalizeReturnUrl(input.ReturnUrl);
                if (!string.IsNullOrWhiteSpace(safeReturn))
                    return Redirect(safeReturn);
            }

            return RedirectAfterPerfilWrite(clientId);
        }

        private string? NormalizeReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return null;

            return Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        }

        private string? NormalizeReturnUrlFromReferer()
        {
            var referer = Request.Headers.Referer.ToString();
            if (string.IsNullOrWhiteSpace(referer))
                return null;

            if (System.Uri.TryCreate(referer, System.UriKind.Absolute, out var uri))
            {
                var candidate = uri.PathAndQuery;
                return Url.IsLocalUrl(candidate) ? candidate : null;
            }

            return Url.IsLocalUrl(referer) ? referer : null;
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        public async Task<IActionResult> ApagarMeta(int id)
        {
            var meta = await _context.Meta
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MetaId == id);

            if (meta == null)
                return NotFound();

            var allowedClientId = await ResolveClientIdForReadAsync(meta.ClientId);
            if (allowedClientId == null)
                return Forbid();

            var vm = new MetaEditViewModel
            {
                MetaId = meta.MetaId,
                ClientId = meta.ClientId,
                MetaDescription = meta.MetaDescription,
                DailyCalories = meta.DailyCalories,
                DailyProtein = meta.DailyProtein,
                DailyFat = meta.DailyFat,
                DailyHydrates = meta.DailyHydrates,
                DailyVitamins = meta.DailyVitamins
            };
            return View(vm);
        }

        [Authorize(Roles = "Nutricionista,Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApagarMetaConfirmado(int metaId, string clientId)
        {
            var resolvedClientId = await ResolveClientIdForWriteAsync(clientId);
            if (resolvedClientId == null)
                return Forbid();

            var meta = await _context.Meta
                .FirstOrDefaultAsync(m => m.MetaId == metaId);

            if (meta == null)
                return NotFound();

            if (!string.Equals(meta.ClientId, resolvedClientId, System.StringComparison.Ordinal))
                return Forbid();

            _context.Meta.Remove(meta);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Meta removida com sucesso.";
            TempData["AlertType"] = "success";
            return RedirectAfterPerfilWrite(resolvedClientId);
        }

        // ---------------------------
        // Helpers (isolamento por role)
        // ---------------------------

        private async Task<Client?> GetCurrentClientAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _context.Client
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentityUserId == userId);
        }

        private async Task<string?> ResolveClientIdForReadAsync(string? requestedClientId)
        {
            if (User.IsInRole("Nutricionista"))
            {
                return string.IsNullOrWhiteSpace(requestedClientId) ? null : requestedClientId;
            }

            if (User.IsInRole("Cliente"))
            {
                var client = await GetCurrentClientAsync();
                return client?.ClientId;
            }

            return null;
        }

        private async Task<string?> ResolveClientIdForWriteAsync(string requestedClientId)
        {
            if (User.IsInRole("Nutricionista"))
            {
                return string.IsNullOrWhiteSpace(requestedClientId) ? null : requestedClientId;
            }

            if (User.IsInRole("Cliente"))
            {
                var client = await GetCurrentClientAsync();
                if (client == null)
                    return null;

                return client.ClientId;
            }

            return null;
        }

        private IActionResult RedirectAfterPerfilWrite(string clientId)
        {
            if (User.IsInRole("Nutricionista"))
                return RedirectToAction(nameof(GerirPerfilAlimentar), new { clientId });

            return RedirectToAction(nameof(MeuPerfilAlimentar));
        }

        private async Task<GerirPerfilAlimentarViewModel> BuildPerfilAlimentarVm(string clientId, string clientName, string clientEmail)
        {
            var alergias = await _context.ClientAlergia
                .AsNoTracking()
                .Where(x => x.ClientId == clientId)
                .Include(x => x.Alergia)
                .OrderBy(x => x.Alergia!.Nome)
                .Select(x => new ItemAssociadoVm
                {
                    Id = x.AlergiaId,
                    Nome = x.Alergia!.Nome,
                    Descricao = x.Alergia!.Descricao
                })
                .ToListAsync();

            var restricoes = await _context.ClientRestricao
                .AsNoTracking()
                .Where(x => x.ClientId == clientId)
                .Include(x => x.RestricaoAlimentar)
                .OrderBy(x => x.RestricaoAlimentar!.Nome)
                .Select(x => new ItemAssociadoVm
                {
                    Id = x.RestricaoAlimentarId,
                    Nome = x.RestricaoAlimentar!.Nome,
                    Descricao = x.RestricaoAlimentar!.Descricao
                })
                .ToListAsync();

            var metas = await _context.Meta
                .AsNoTracking()
                .Where(m => m.ClientId == clientId)
                .OrderByDescending(m => m.MetaId)
                .Select(m => new MetaResumoVm
                {
                    MetaId = m.MetaId,
                    MetaDescription = m.MetaDescription,
                    DailyCalories = m.DailyCalories,
                    DailyProtein = m.DailyProtein,
                    DailyFat = m.DailyFat,
                    DailyHydrates = m.DailyHydrates,
                    DailyVitamins = m.DailyVitamins
                })
                .ToListAsync();

            var opcoesAlergias = await _context.Alergia
                .AsNoTracking()
                .OrderBy(a => a.Nome)
                .Select(a => new SelectOptionIntVm
                {
                    Value = a.AlergiaId,
                    Text = a.Descricao != null && a.Descricao != "" ? $"{a.Nome} — {a.Descricao}" : a.Nome
                })
                .ToListAsync();

            var opcoesRestricoes = await _context.RestricaoAlimentar
                .AsNoTracking()
                .OrderBy(r => r.Nome)
                .Select(r => new SelectOptionIntVm
                {
                    Value = r.RestricaoAlimentarId,
                    Text = r.Descricao != null && r.Descricao != "" ? $"{r.Nome} — {r.Descricao}" : r.Nome
                })
                .ToListAsync();

            return new GerirPerfilAlimentarViewModel
            {
                ClientId = clientId,
                ClientNome = clientName,
                ClientEmail = clientEmail,
                Alergias = alergias,
                Restricoes = restricoes,
                Metas = metas,
                OpcoesAlergias = opcoesAlergias,
                OpcoesRestricoes = opcoesRestricoes
            };
        }
    }
}
