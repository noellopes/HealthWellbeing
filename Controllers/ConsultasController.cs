using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            if (!User.IsInRole("DiretorClinico"))
            {
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
        public async Task<IActionResult> Marcar(int? idEspecialidade, int? idMedico)
        {
            var vm = new MarcarConsultaGridVM
            {
                IdEspecialidade = idEspecialidade,
                IdMedico = idMedico,
                Especialidades = await _context.Specialities
                    .OrderBy(e => e.Nome)
                    .Select(e => new SimpleOptionVM { Id = e.IdEspecialidade, Nome = e.Nome })
                    .ToListAsync()
            };

            if (idEspecialidade.HasValue)
            {
                vm.Medicos = await _context.Doctor
                    .Where(d => d.IdEspecialidade == idEspecialidade.Value)
                    .OrderBy(d => d.Nome)
                    .Select(d => new SimpleOptionVM { Id = d.IdMedico, Nome = d.Nome })
                    .ToListAsync();
            }

            if (idMedico.HasValue)
            {
                await FillGrid(vm, idMedico.Value);
            }

            return View(vm);
        }

        // POST: /Consultas/MarcarSlot  (clicar em "Livre")
        [Authorize(Roles = "Utente,Rececionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarSlot(int idMedico, int idEspecialidade, string data, string horaInicio, string horaFim)
        {
            var d = DateOnly.ParseExact(data, "yyyy-MM-dd");
            var hi = TimeOnly.ParseExact(horaInicio, "HH:mm");
            var hf = TimeOnly.ParseExact(horaFim, "HH:mm");

            // 🔎 obter o utente logado (ajusta ao teu projeto)
            var email = User.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return Content("Utilizador sem email no Identity.");

            var utente = await _context.UtenteSaude
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);

            if (utente == null)
                return Content("Este utilizador não é Utente (não existe UtenteSaude associado ao Client).");
           
            // ✅ LIMITAR A 2 CONSULTAS POR DIA (não canceladas)
            var totalNoDia = await _context.Consulta.CountAsync(c =>
                c.IdUtenteSaude == utente.UtenteSaudeId &&              
                !c.DataCancelamento.HasValue &&
                DateOnly.FromDateTime(c.DataConsulta) == d
            );

            if (totalNoDia >= 2)
            {
                TempData["Erro"] = "Só podes marcar no máximo 2 consultas por dia.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade, idMedico });
            }

            var inicioConsulta = d.ToDateTime(hi);
            if (inicioConsulta <= DateTime.Now)
            {
                TempData["Erro"] = "Não é possível marcar consultas no passado.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade, idMedico });
            }

            // validar se slot está dentro do horário do médico
            var bloco = await _context.AgendaMedica.FirstOrDefaultAsync(a =>
                a.IdMedico == idMedico &&
                a.Data == d &&
                a.HoraInicio <= hi &&
                a.HoraFim >= hf
            );

            if (bloco == null)
            {
                TempData["Erro"] = "O slot escolhido não está dentro do horário de trabalho do médico.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade, idMedico });
            }

            // validar conflito do médico (não canceladas)
            bool existeConflito = await _context.Consulta.AnyAsync(c =>
                c.IdMedico == idMedico &&
                !c.DataCancelamento.HasValue &&
                DateOnly.FromDateTime(c.DataConsulta) == d &&
                hi < c.HoraFim && hf > c.HoraInicio
            );

            if (existeConflito)
            {
                TempData["Erro"] = "Esse horário já foi marcado entretanto. Escolhe outro slot.";
                return RedirectToAction(nameof(Marcar), new { idEspecialidade, idMedico });
            }

            var consulta = new Consulta
            {
                IdMedico = idMedico,
                IdEspecialidade = idEspecialidade,

                IdUtenteSaude = utente.UtenteSaudeId, 
                DataMarcacao = DateTime.Now,

                // melhor guardar a data com a hora real da consulta:
                DataConsulta = d.ToDateTime(hi),
                HoraInicio = hi,
                HoraFim = hf
            };

            _context.Consulta.Add(consulta);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Consulta marcada com sucesso!";
            return RedirectToAction(nameof(Marcar), new { idEspecialidade, idMedico });
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

            // ✅ Segurança básica: Utente só vê as suas, Médico só vê as suas
            if (User.IsInRole("Utente"))
            {
                var email = User.Identity?.Name?.Trim();
                if (string.IsNullOrWhiteSpace(email))
                    return Content("Utilizador sem email no Identity.");

                var utente = await _context.UtenteSaude
                    .Include(u => u.Client)
                    .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);

                if (utente == null)
                    return Content("Este utilizador não é Utente (não existe UtenteSaude associado ao Client).");
            }

            if (User.IsInRole("Medico"))
            {
                // se tiveres o email do médico no Identity = Doctor.Email
                var email = User.Identity?.Name?.Trim();
                if (string.IsNullOrWhiteSpace(email))
                    return Content("Utilizador sem email no Identity.");

                var utente = await _context.UtenteSaude
                    .Include(u => u.Client)
                    .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);

                if (utente == null)
                    return Content("Este utilizador não é Utente (não existe UtenteSaude associado ao Client).");
            }

            return View(consulta);
        }
    }
}
