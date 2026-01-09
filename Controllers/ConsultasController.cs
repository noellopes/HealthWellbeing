using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsultasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private const string SessUtenteId = "SelectedUtenteSaudeId";
        private const string SessUtenteEmail = "SelectedUtenteEmail";
        private const string SessUtenteNome = "SelectedUtenteNome";

        private async Task<UtenteSaude?> GetUtenteSelecionadoRececionistaAsync()
        {
            var id = HttpContext.Session.GetInt32(SessUtenteId);
            if (!id.HasValue) return null;

            return await _context.UtenteSaude
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.UtenteSaudeId == id.Value);
        }
        //
        [HttpGet]
        public async Task<IActionResult> Observacoes(int id)
        {
            var consulta = await _context.Consulta
                .FirstOrDefaultAsync(c => c.IdConsulta == id);

            if (consulta == null)
                return NotFound();

            var vm = new ConsultaObservacoesVM
            {
                IdConsulta = consulta.IdConsulta,
                Observacoes = consulta.Observacoes
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Observacoes(ConsultaObservacoesVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var consulta = await _context.Consulta
                .FirstOrDefaultAsync(c => c.IdConsulta == vm.IdConsulta);

            if (consulta == null)
                return NotFound();

            consulta.Observacoes = vm.Observacoes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Observações guardadas com sucesso.";

            return RedirectToAction("Details", new { id = vm.IdConsulta });
        }

        [Authorize(Roles = "DiretorClinico,Administrador")]
        [HttpGet]
        public async Task<IActionResult> Relatorio()
        {
            var hoje = DateTime.Today;

            var totalMarcadas = await _context.Consulta.CountAsync();

            var totalCanceladas = await _context.Consulta
                .CountAsync(c => c.DataCancelamento.HasValue);

            var totalNaoCanceladas = await _context.Consulta
                .CountAsync(c => !c.DataCancelamento.HasValue);

            // Realizadas: já passaram, não canceladas e têm observações
            var totalRealizadas = await _context.Consulta
                .CountAsync(c =>
                    !c.DataCancelamento.HasValue &&
                    c.DataConsulta.Date < hoje &&
                    !string.IsNullOrWhiteSpace(c.Observacoes)
                );

            // Faltadas: já passaram, não canceladas e NÃO têm observações
            var totalFaltadas = await _context.Consulta
                .CountAsync(c =>
                    !c.DataCancelamento.HasValue &&
                    c.DataConsulta.Date < hoje &&
                    string.IsNullOrWhiteSpace(c.Observacoes)
                );

            var vm = new RelatorioConsultasVM
            {
                TotalMarcadas = totalMarcadas,
                TotalCanceladas = totalCanceladas,
                TotalNaoCanceladas = totalNaoCanceladas,
                TotalRealizadas = totalRealizadas,
                TotalFaltadas = totalFaltadas
            };

            return View(vm);
        }

        // -------------------------------
        // LISTA / HISTÓRICO (mantido)
        // -------------------------------
        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {

            var hoje = DateTime.Today;

            var consultasQuery = _context.Consulta
                .Include(c => c.Doctor)
                .Include(c => c.Speciality)
                .AsQueryable();

            if (User.IsInRole("Utente"))
            {
                // Utente so ve as suas consultas
                var utente = await GetUtenteFromLoggedUser();
                if (utente == null)
                    return Content("Não existe um utente associado ao teu utilizador (email não encontrado em UtenteSaude).");

                consultasQuery = consultasQuery.Where(c => c.IdUtenteSaude == utente.UtenteSaudeId);
            }
            else if (User.IsInRole("Medico"))
            {
                // Médico só vê as suas consultas
                var medico = await GetDoctorFromLoggedUser();
                if (medico == null)
                    return Content("Não existe um médico associado ao teu utilizador (email não encontrado em Doctor).");
                consultasQuery = consultasQuery.Where(c => c.IdMedico == medico.IdMedico);
            }
            else if (!User.IsInRole("DiretorClinico"))
            {
                // Médico / Rececionista: só consultas futuras não canceladas
                consultasQuery = consultasQuery.Where(c =>
                    !c.DataCancelamento.HasValue &&
                    c.DataConsulta >= DateTime.Now
                );
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var culture = new CultureInfo("pt-PT");
                var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };

                if (DateTime.TryParseExact(searchTerm.Trim(), formats, culture, DateTimeStyles.None, out var dataPesquisa))
                {
                    consultasQuery = consultasQuery.Where(c =>
                        c.DataMarcacao.Date == dataPesquisa.Date ||
                        c.DataConsulta.Date == dataPesquisa.Date
                    );
                }
                else
                {
                    consultasQuery = consultasQuery.Where(c => false);
                }
            }

            int numberConsultas = await consultasQuery.CountAsync();
            var consultasInfo = new PaginationInfo<Consulta>(page, numberConsultas);
            var pageSize = consultasInfo.ItemsPerPage > 0 ? consultasInfo.ItemsPerPage : 10;

            consultasInfo.Items = await consultasQuery
                .OrderByDescending(c => c.DataConsulta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View(consultasInfo);
        }

        // ----------------------------------------------------
        // MARCAÇÃO (NOVA VERSÃO: grelha server-side simples)
        // ----------------------------------------------------

        // GET: /Consultas/Marcar?idEspecialidade=..&idMedico=..
        [Authorize(Roles = "Utente,Rececionista")]
        [HttpGet]
        public async Task<IActionResult> Marcar(int? idEspecialidade, int? idMedico, bool naoSelecionarMedico = false)
        {
            // ✅ Rececionista tem de ter utente selecionado
            if (User.IsInRole("Rececionista"))
            {
                var utSel = await GetUtenteSelecionadoRececionistaAsync();
                if (utSel == null)
                {
                    TempData["Erro"] = "Escolhe primeiro um utente (clica em Detalhes) antes de marcares a consulta.";
                    return RedirectToAction("RecepcionistaView", "UtenteSaude");
                }

                ViewBag.SelectedUtenteNome = HttpContext.Session.GetString(SessUtenteNome) ?? utSel.Client?.Name ?? utSel.NomeCompleto ?? "—";
                ViewBag.SelectedUtenteEmail = HttpContext.Session.GetString(SessUtenteEmail) ?? utSel.Client?.Email ?? "—";
            }

            var vm = new MarcarConsultaGridVM
            {
                IdEspecialidade = idEspecialidade,
                IdMedico = idMedico,
                NaoSelecionarMedico = naoSelecionarMedico,
                Especialidades = await _context.Specialities
                    .OrderBy(e => e.Nome)
                    .Select(e => new SimpleOptionVM { Id = e.IdEspecialidade, Nome = e.Nome })
                    .ToListAsync()
            };
            //var vm = new MarcarConsultaGridVM
           /* {
                IdEspecialidade = idEspecialidade,
                IdMedico = idMedico,
                NaoSelecionarMedico = naoSelecionarMedico,
                Especialidades = await _context.Specialities
                    .OrderBy(e => e.Nome)
                    .Select(e => new SimpleOptionVM { Id = e.IdEspecialidade, Nome = e.Nome })
                    .ToListAsync()
            };*/

            if (idEspecialidade.HasValue)
            {
                vm.Medicos = await _context.Doctor
                    .Where(d => d.IdEspecialidade == idEspecialidade.Value)
                    .OrderBy(d => d.Nome)
                    .Select(d => new SimpleOptionVM { Id = d.IdMedico, Nome = d.Nome })
                    .ToListAsync();
            }
            if (naoSelecionarMedico && idEspecialidade.HasValue ){
                await FillGridPreferencial(vm, idEspecialidade.Value);
            }
            else if (idMedico.HasValue)
            {
                await FillGrid(vm, idMedico.Value);
            }

            return View(vm);
        }

        // POST: /Consultas/MarcarSlot  (clicar em "Livre")
        [Authorize(Roles = "Utente,Rececionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarConsulta(MarcarConsultaGridVM vm)
        {
            // validações básicas
            if (!vm.IdEspecialidade.HasValue)
            {
                TempData["Erro"] = "Escolhe a especialidade.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
            }

            if (string.IsNullOrWhiteSpace(vm.DataSelecionada))
            {
                TempData["Erro"] = "Escolhe um horário na grelha.";
                return RedirectToAction(nameof(Marcar), new
                {
                    idEspecialidade = vm.IdEspecialidade,
                    idMedico = vm.IdMedico,
                    naoSelecionarMedico = vm.NaoSelecionarMedico
                });
            }

            var partes = vm.DataSelecionada.Split('|');
            if (partes.Length != 3)
            {
                TempData["Erro"] = "Horário inválido. Tenta novamente.";
                return RedirectToAction(nameof(Marcar), new
                {
                    idEspecialidade = vm.IdEspecialidade,
                    idMedico = vm.IdMedico,
                    naoSelecionarMedico = vm.NaoSelecionarMedico
                });
            }

            var d = DateOnly.ParseExact(partes[0], "yyyy-MM-dd");
            var hi = TimeOnly.ParseExact(partes[1], "HH:mm");
            var hf = TimeOnly.ParseExact(partes[2], "HH:mm");

            var inicioConsultaGlobal = d.ToDateTime(hi);
            if (inicioConsultaGlobal <= DateTime.Now)
            {
                TempData["Erro"] = "Não é possível marcar consultas no passado.";
                return RedirectToAction(nameof(Marcar), new
                {
                    idEspecialidade = vm.IdEspecialidade,
                    idMedico = vm.IdMedico,
                    naoSelecionarMedico = vm.NaoSelecionarMedico
                });
            }
            // 🔎 obter o utente (depende do role)
            UtenteSaude? utente = null;

            if (User.IsInRole("Utente"))
            {
                utente = await GetUtenteFromLoggedUser();
                if (utente == null)
                    return Content("Não existe um utente associado ao teu utilizador (email não encontrado em UtenteSaude).");
            }
            else if (User.IsInRole("Rececionista"))
            {
                utente = await GetUtenteSelecionadoRececionistaAsync();
                if (utente == null)
                {
                    TempData["Erro"] = "Escolhe primeiro um utente (clica em Detalhes) antes de confirmares a marcação.";
                    return RedirectToAction("RecepcionistaView", "UtenteSaude");
                }
            }
            else
            {
                return Forbid();
            }
            /*
            // 🔎 obter o utente logado (ajusta ao teu projeto)
            var email = User.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return Content("Utilizador sem email no Identity.");

            var utente = await _context.UtenteSaude
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);

            if (utente == null)
                return Content("Não existe um utente associado ao teu utilizador (email não encontrado em UtenteSaude).");
            */
            // limite 2 consultas/dia (igual ao teu)
            var totalNoDia = await _context.Consulta.CountAsync(c =>
                c.IdUtenteSaude == utente.UtenteSaudeId &&
                !c.DataCancelamento.HasValue &&
                DateOnly.FromDateTime(c.DataConsulta) == d
            );

            if (totalNoDia >= 2)
            {
                TempData["Erro"] = "Só podes marcar no máximo 2 consultas por dia.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
            }

            int idEspecialidade = vm.IdEspecialidade.Value;
            int? idMedicoFinal = null;

            // ----------- ESCOLHA DO MÉDICO -----------
            if (vm.NaoSelecionarMedico)
            {
                // marcação preferencial
                idMedicoFinal = await EscolherMedicoPreferencialAsync(idEspecialidade, d, hi, hf);

                if (!idMedicoFinal.HasValue)
                {
                    TempData["Erro"] = "Não há médicos disponíveis nesse horário.";
                    return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
                }
            }
            else
            {
                // marcação normal: exige médico escolhido
                if (!vm.IdMedico.HasValue)
                {
                    TempData["Erro"] = "Escolhe um médico ou marca a opção 'Não selecionar o médico'.";
                    return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
                }

                idMedicoFinal = vm.IdMedico.Value;
                var inicioConsulta = d.ToDateTime(hi);
                if (inicioConsulta <= DateTime.Now)
                {
                    TempData["Erro"] = "Não é possível marcar consultas no passado.";
                    return RedirectToAction(nameof(Marcar), new { idEspecialidade, vm.IdMedico.Value });
                } 

                // validar se slot está dentro da agenda do médico
                var bloco = await _context.AgendaMedica.FirstOrDefaultAsync(a =>
                    a.IdMedico == idMedicoFinal.Value &&
                    a.Data == d &&
                    a.HoraInicio <= hi &&
                    a.HoraFim >= hf
                );

                if (bloco == null)
                {
                    TempData["Erro"] = "O slot escolhido não está dentro do horário de trabalho do médico.";
                    return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
                }

                // conflito do médico (não canceladas)
                bool existeConflito = await _context.Consulta.AnyAsync(c =>
                    c.IdMedico == idMedicoFinal.Value &&
                    !c.DataCancelamento.HasValue &&
                    DateOnly.FromDateTime(c.DataConsulta) == d &&
                    hi < c.HoraFim && hf > c.HoraInicio
                );

                if (existeConflito)
                {
                    TempData["Erro"] = "Esse horário já foi marcado entretanto. Escolhe outro slot.";
                    return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
                }
            }

            // (recomendado) transação + revalidação rápida para evitar corridas
            using var tx = await _context.Database.BeginTransactionAsync();

            bool aindaLivre = !await _context.Consulta.AnyAsync(c =>
                c.IdMedico == idMedicoFinal.Value &&
                !c.DataCancelamento.HasValue &&
                DateOnly.FromDateTime(c.DataConsulta) == d &&
                hi < c.HoraFim && hf > c.HoraInicio
            );

            if (!aindaLivre)
            {
                await tx.RollbackAsync();
                TempData["Erro"] = "O horário foi ocupado no último momento. Tenta novamente.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
            }

            var consulta = new Consulta
            {
                IdMedico = idMedicoFinal.Value,
                IdEspecialidade = idEspecialidade,
                IdUtenteSaude = utente.UtenteSaudeId,
                DataMarcacao = DateTime.Now,
                DataConsulta = d.ToDateTime(hi),
                HoraInicio = hi,
                HoraFim = hf
            };

            _context.Consulta.Add(consulta);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            if (User.IsInRole("Rececionista"))
            {
                HttpContext.Session.Remove(SessUtenteId);
                HttpContext.Session.Remove(SessUtenteEmail);
                HttpContext.Session.Remove(SessUtenteNome);
            }

            //  TempData["Ok"] = "Consulta marcada com sucesso!";
            // return RedirectToAction(nameof(Marcar), new { idEspecialidade = vm.IdEspecialidade, idMedico = vm.IdMedico });
            if (User.IsInRole("Rececionista"))
            {
                HttpContext.Session.Remove("SelectedUtenteSaudeId");
                HttpContext.Session.Remove("SelectedUtenteEmail");
                HttpContext.Session.Remove("SelectedUtenteNome");

                TempData["Msg"] = "Consulta marcada com sucesso!";
                return RedirectToAction("RecepcionistaView", "UtenteSaude");
            }

            TempData["Ok"] = "Consulta marcada com sucesso!";
            return RedirectToAction(nameof(Marcar), new
            {
                idEspecialidade = vm.IdEspecialidade,
                idMedico = vm.IdMedico,
                naoSelecionarMedico = vm.NaoSelecionarMedico
            });

        }


        // ----------------- helpers -----------------

        private async Task FillGrid(MarcarConsultaGridVM vm, int idMedico)
        {
            vm.Datas = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);
         
            // IMPORTANTE:
            // - busca agenda por Data (correto)
            // - E também busca "templates" antigos (Seed) com Data default, para fallback por DiaSemana
            var agenda = await _context.AgendaMedica
                .Where(a => a.IdMedico == idMedico &&
                            (vm.Datas.Contains(a.Data) || a.Data == default(DateOnly)))
                .ToListAsync();

            var consultas = await _context.Consulta
               .Where(c => c.IdMedico == idMedico
               && !c.DataCancelamento.HasValue
               && vm.Datas.Contains(DateOnly.FromDateTime(c.DataConsulta)))
              .Select(c => new
              {
                  Data = DateOnly.FromDateTime(c.DataConsulta),
                  c.HoraInicio,
                  c.HoraFim
              })
              .ToListAsync();

            var duracao = TimeSpan.FromMinutes(30);

            // construir slots possíveis com base na agenda (por Data OU por DiaSemana fallback)
            var allSlots = new SortedSet<(TimeOnly start, TimeOnly end)>();

            foreach (var dia in vm.Datas)
            {
                var blocosDoDia = agenda
                    .Where(a => a.Data == dia || (a.Data == default(DateOnly) && a.DiaSemana == dia.DayOfWeek))
                    .ToList();

                foreach (var bloco in blocosDoDia)
                {
                    var ini = bloco.HoraInicio.ToTimeSpan();
                    var fim = bloco.HoraFim.ToTimeSpan();

                    for (var t = ini; t + duracao <= fim; t += duracao)
                    {
                        var s = TimeOnly.FromTimeSpan(t);
                        var e = TimeOnly.FromTimeSpan(t + duracao);
                        allSlots.Add((s, e));
                    }
                }
            }

            // montar grelha
            foreach (var (s, e) in allSlots)
            {
                var row = new TimeSlotRowVM { Inicio = s, Fim = e };

                foreach (var dia in vm.Datas)
                {
                    var blocosDoDia = agenda
                        .Where(a => a.Data == dia || (a.Data == default(DateOnly) && a.DiaSemana == dia.DayOfWeek))
                        .ToList();

                    bool dentroHorario = blocosDoDia.Any(a => a.HoraInicio <= s && a.HoraFim >= e);

                    if (!dentroHorario)
                    {
                        row.Cells.Add(new TimeSlotCellVM
                        {
                            Data = dia,
                            Inicio = s,
                            Fim = e,
                            Estado = "fora"
                        });
                        continue;
                    }

                    bool ocupado = consultas.Any(c =>
                        c.Data == dia &&
                        Overlaps(s, e, c.HoraInicio, c.HoraFim)
                    );

                    row.Cells.Add(new TimeSlotCellVM
                    {
                        Data = dia,
                        Inicio = s,
                        Fim = e,
                        Estado = ocupado ? "ocupado" : "livre"
                    });
                }

                vm.Linhas.Add(row);
            }
        }

        private async Task<UtenteSaude?> GetUtenteFromLoggedUser()
        {
            var email = User?.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.UtenteSaude
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);
        }

        private async Task<Doctor?> GetDoctorFromLoggedUser()
        {
            var email = User?.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email == email);
        }

        private async Task FillGridPreferencial(MarcarConsultaGridVM vm, int idEspecialidade)
        {
            vm.Datas = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            var idsMedicos = await _context.Doctor
                .Where(d => d.IdEspecialidade == idEspecialidade)
                .Select(d => d.IdMedico)
                .ToListAsync();

            if (idsMedicos.Count == 0) return;

            // carregar agenda dos médicos da especialidade (para as datas)
            var agenda = await _context.AgendaMedica
                .Where(a => idsMedicos.Contains(a.IdMedico.Value) && vm.Datas.Contains(a.Data))
                .ToListAsync();

            // carregar consultas (não canceladas) desses médicos (para as datas)
            var consultas = await _context.Consulta
                .Where(c => idsMedicos.Contains(c.IdMedico)
                            && !c.DataCancelamento.HasValue
                            && vm.Datas.Contains(DateOnly.FromDateTime(c.DataConsulta)))
                .Select(c => new
                {
                    c.IdMedico,
                    Data = DateOnly.FromDateTime(c.DataConsulta),
                    c.HoraInicio,
                    c.HoraFim
                })
                .ToListAsync();

            var duracao = TimeSpan.FromMinutes(30);

            // construir slots (união de todos os blocos da especialidade)
            var allSlots = new SortedSet<(TimeOnly start, TimeOnly end)>();

            foreach (var dia in vm.Datas)
            {
                var blocos = agenda.Where(a => a.Data == dia).ToList();
                foreach (var b in blocos)
                {
                    var ini = b.HoraInicio.ToTimeSpan();
                    var fim = b.HoraFim.ToTimeSpan();

                    for (var t = ini; t + duracao <= fim; t += duracao)
                    {
                        allSlots.Add((TimeOnly.FromTimeSpan(t), TimeOnly.FromTimeSpan(t + duracao)));
                    }
                }
            }

            foreach (var (s, e) in allSlots)
            {
                var row = new TimeSlotRowVM { Inicio = s, Fim = e };

                foreach (var dia in vm.Datas)
                {
                    // médicos que trabalham nesse dia e cujo bloco cobre o slot
                    var medicosEmTrabalho = agenda
                        .Where(a => a.Data == dia && a.HoraInicio <= s && a.HoraFim >= e)
                        .Select(a => a.IdMedico)
                        .Distinct()
                        .ToList();

                    if (medicosEmTrabalho.Count == 0)
                    {
                        row.Cells.Add(new TimeSlotCellVM { Data = dia, Inicio = s, Fim = e, Estado = "fora" });
                        continue;
                    }

                    // existe pelo menos 1 médico sem conflito?
                    bool existeDisponivel = medicosEmTrabalho.Any(idMedico =>
                        !consultas.Any(c =>
                            c.IdMedico == idMedico &&
                            c.Data == dia &&
                            Overlaps(s, e, c.HoraInicio, c.HoraFim)
                        )
                    );

                    row.Cells.Add(new TimeSlotCellVM
                    {
                        Data = dia,
                        Inicio = s,
                        Fim = e,
                        Estado = existeDisponivel ? "livre" : "ocupado"
                    });
                }

                vm.Linhas.Add(row);
            }
        }


        private async Task<int?> EscolherMedicoPreferencialAsync(int idEspecialidade, DateOnly d, TimeOnly hi, TimeOnly hf)
        {
            // 1) médicos da especialidade
            var medicos = await _context.Doctor
                .Where(m => m.IdEspecialidade == idEspecialidade)
                .Select(m => new { m.IdMedico, m.Nome })
                .ToListAsync();

            if (medicos.Count == 0) return null;

            var ids = medicos.Select(m => m.IdMedico).ToList();

            // 2) médicos a trabalhar nesse dia/hora (agenda cobre o slot)
            var idsEmTrabalho = await _context.AgendaMedica
                .Where(a =>
                    a.IdMedico.HasValue &&
                    ids.Contains(a.IdMedico.Value) &&
                    a.Data == d &&
                    a.HoraInicio <= hi &&
                    a.HoraFim >= hf
                )
                .Select(a => a.IdMedico.Value)
                .Distinct()
                .ToListAsync();

            if (idsEmTrabalho.Count == 0) return null;

            // 3) remover médicos com conflito nesse slot
            var idsComConflito = await _context.Consulta
                .Where(c =>
                    idsEmTrabalho.Contains(c.IdMedico) &&
                    !c.DataCancelamento.HasValue &&
                    DateOnly.FromDateTime(c.DataConsulta) == d &&
                    hi < c.HoraFim && hf > c.HoraInicio
                )
                .Select(c => c.IdMedico)
                .Distinct()
                .ToListAsync();

            var disponiveis = idsEmTrabalho.Except(idsComConflito).ToList();
            if (disponiveis.Count == 0) return null;

            // 4) contar consultas no dia (balanceamento)
            var contagens = await _context.Consulta
                .Where(c =>
                    disponiveis.Contains(c.IdMedico) &&
                    !c.DataCancelamento.HasValue &&
                    DateOnly.FromDateTime(c.DataConsulta) == d
                )
                .GroupBy(c => c.IdMedico)
                .Select(g => new { IdMedico = g.Key, Total = g.Count() })
                .ToListAsync();

            int TotalNoDia(int idMedico) => contagens.FirstOrDefault(x => x.IdMedico == idMedico)?.Total ?? 0;

            // 5) escolher: menos consultas no dia, empate por nome (alfabético)
            var escolhido = medicos
                .Where(m => disponiveis.Contains(m.IdMedico))
                .OrderBy(m => TotalNoDia(m.IdMedico))
                .ThenBy(m => m.Nome) // alfabético
                .FirstOrDefault();

            return escolhido?.IdMedico;
        }

        private static bool Overlaps(TimeOnly aStart, TimeOnly aEnd, TimeOnly bStart, TimeOnly bEnd)
        {
            return aStart < bEnd && bStart < aEnd;
        }

        private static List<DateOnly> GetProximosDiasUteis(DateOnly inicio, int quantidade)
        {
            var res = new List<DateOnly>();
            var d = inicio;

            while (res.Count < quantidade)
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    res.Add(d);

                d = d.AddDays(1);
            }

            return res;
        }
        [Authorize(Roles = "Utente,Medico,Rececionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var consulta = await _context.Consulta.FirstOrDefaultAsync(c => c.IdConsulta == id);
            if (consulta == null) return NotFound();

            // se já estiver cancelada, não faz nada
            if (!consulta.DataCancelamento.HasValue)
            {
                consulta.DataCancelamento = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Consulta cancelada e o horário ficou disponível.";
            return RedirectToAction(nameof(Index));

            // Utente só cancela as suas consultas
            if (User.IsInRole("Utente"))
            {
                var utente = await GetUtenteFromLoggedUser();
                if (utente == null || consulta.IdUtenteSaude != utente.UtenteSaudeId)
                    return Forbid();
            }

            // Médico só cancela as suas consultas

            if (!consulta.DataCancelamento.HasValue)
            {
                consulta.DataCancelamento = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Consulta cancelada e o horário ficou disponível.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Utente,Medico,Rececionista,DiretorClinico")]
        public async Task<IActionResult> Details(int id)
        {
            var consulta = await _context.Consulta
                .Include(c => c.Doctor)
                .Include(c => c.Speciality)
                .Include(c => c.UtenteSaude)
                    .ThenInclude(u => u.Client)
                .FirstOrDefaultAsync(c => c.IdConsulta == id);

            if (consulta == null) return NotFound();

            // Utente só vê as próprias
            if (User.IsInRole("Utente"))
            {
                var utente = await GetUtenteFromLoggedUser();
                if (utente == null)
                    return Content("Não existe um utente associado ao teu utilizador.");

                if (consulta.IdUtenteSaude != utente.UtenteSaudeId)
                    return Forbid(); // 403
            }

            // Médico só vê as suas
            if (User.IsInRole("Medico"))
            {
                var doctor = await GetDoctorFromLoggedUser(); // igual ao que tens noutro controller
                if (doctor == null)
                    return Content("Não existe um médico associado ao teu utilizador.");

                if (consulta.IdMedico != doctor.IdMedico)
                    return Forbid();
            }

            if (User.IsInRole("Medico"))
                return View(consulta);

            return View(consulta);
        }
    }
}
