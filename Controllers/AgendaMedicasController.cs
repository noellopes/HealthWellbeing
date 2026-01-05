using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class AgendaMedicasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AgendaMedicasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /AgendaMedicas
        public async Task<IActionResult> Index()
        {
            var datas15Uteis = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            var agenda = await _context.AgendaMedica
                .Where(a => datas15Uteis.Contains(a.Data))
                .Include(a => a.Medico)
                    .ThenInclude(m => m.Especialidade)
                .ToListAsync();

            // Agrupar por especialidade
            var model = agenda
                .Where(a => a.Medico != null && a.Medico.Especialidade != null)
                .GroupBy(a => a.Medico!.Especialidade!.Nome)
                .Select(espGroup => new EspecialidadeAgendaVM
                {
                    Especialidade = espGroup.Key,
                    Datas = datas15Uteis,

                    Medicos = espGroup
                        .GroupBy(a => a.Medico!)
                        .Select(medGroup => new MedicoAgendaTabelaVM
                        {
                            Medico = medGroup.Key.Nome,
                            HorariosPorData = datas15Uteis.ToDictionary(
                                d => d,
                                d => FormatDia(medGroup.Where(x => x.Data == d).ToList())
                            )
                        })
                        .OrderBy(m => m.Medico)
                        .ToList()
                })
                .OrderBy(e => e.Especialidade)
                .ToList();

            return View(model);
        }

        // GET: /AgendaMedicas/MinhaAgenda15Dias
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> MinhaAgenda15Dias()
        {
            var doctor = await GetDoctorFromLoggedUser();
            if (doctor == null)
                return Content("Não existe um médico associado ao teu utilizador (email não encontrado em Doctor).");

            var datas15Uteis = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            // Carregar o que já existe no período
            var existentes = await _context.AgendaMedica
                .Where(a => a.IdMedico == doctor.IdMedico && datas15Uteis.Contains(a.Data))
                .ToListAsync();

            var vm = new MedicoAgenda15DiasVM
            {
                Medico = doctor.Nome,
                Dias = datas15Uteis.Select(d =>
                {
                    var diaEntries = existentes.Where(x => x.Data == d).ToList();

                    var manha = diaEntries.FirstOrDefault(x => x.Periodo == "Manha");
                    var tarde = diaEntries.FirstOrDefault(x => x.Periodo == "Tarde");

                    return new AgendaDiaVM
                    {
                        Data = d,
                        DiaSemana = d.DayOfWeek,
                        Manha = manha != null
                            ? new MedicoAgendaVM
                            {
                                Periodo = "Manha",
                                Ativo = true,
                                HoraInicio = manha.HoraInicio,
                                HoraFim = manha.HoraFim
                            }
                            : new MedicoAgendaVM
                            {
                                Periodo = "Manha",
                                Ativo = false
                            },
                        Tarde = tarde != null
                            ? new MedicoAgendaVM
                            {
                                Periodo = "Tarde",
                                Ativo = true,
                                HoraInicio = tarde.HoraInicio,
                                HoraFim = tarde.HoraFim
                            }
                            : new MedicoAgendaVM
                            {
                                Periodo = "Tarde",
                                Ativo = false
                            }
                    };
                }).ToList()
            };

            return View(vm);
        }

        // POST: /AgendaMedicas/MinhaAgenda15Dias
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Medico")]
        public async Task<IActionResult> MinhaAgenda15Dias(MedicoAgenda15DiasVM vm)
        {
            var doctor = await GetDoctorFromLoggedUser();
            if (doctor == null)
                return Content("Não existe um médico associado ao teu utilizador (email não encontrado em Doctor).");

            if (!ModelState.IsValid)
                return View(vm);

            var datas15Uteis = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            foreach (var dia in vm.Dias.Where(d => datas15Uteis.Contains(d.Data)))
            {
                // manhã
                await UpsertPeriodo(doctor.IdMedico, dia.Data, "Manha", dia.Manha);

                // tarde
                await UpsertPeriodo(doctor.IdMedico, dia.Data, "Tarde", dia.Tarde);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MinhaAgenda15Dias));
        }

        private async Task UpsertPeriodo(int idMedico, DateOnly data, string periodo, MedicoAgendaVM? p)
        {
            var existente = await _context.AgendaMedica.FirstOrDefaultAsync(a =>
                a.IdMedico == idMedico &&
                a.Data == data &&
                a.Periodo == periodo
            );

            // se não há VM ou está desativado -> remove se existir
            if (p == null || !p.Ativo)
            {
                if (existente != null)
                    _context.AgendaMedica.Remove(existente);

                return;
            }

            // validação hora
            if (p.HoraFim <= p.HoraInicio)
            {
                ModelState.AddModelError("", $"Hora fim tem de ser posterior à hora início em {data:dd/MM/yyyy} ({periodo}).");
                return;
            }

            if (existente == null)
            {
                _context.AgendaMedica.Add(new AgendaMedica
                {
                    IdMedico = idMedico,
                    Data = data,
                    DiaSemana = data.DayOfWeek, // só visual
                    Periodo = periodo,          // "Manha" / "Tarde"
                    HoraInicio = p.HoraInicio,
                    HoraFim = p.HoraFim
                });
            }
            else
            {
                existente.HoraInicio = p.HoraInicio;
                existente.HoraFim = p.HoraFim;
                existente.DiaSemana = data.DayOfWeek;
                existente.Periodo = periodo;
            }
        }


        // ----------------- helpers -----------------

        private async Task<Doctor?> GetDoctorFromLoggedUser()
        {
            var email = User?.Identity?.Name?.Trim();
            if (string.IsNullOrWhiteSpace(email)) return null;

            var emailLower = email.ToLower();

            return await _context.Doctor
                .FirstOrDefaultAsync(d => d.Email.ToLower() == emailLower);
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

        private static string FormatDia(List<AgendaMedica> entries)
        {
            var manha = entries.FirstOrDefault(x => x.Periodo == "Manha");
            var tarde = entries.FirstOrDefault(x => x.Periodo == "Tarde");

            var partes = new List<string>();

            if (manha != null)
                partes.Add($"Manhã: {manha.HoraInicio:HH\\:mm}-{manha.HoraFim:HH\\:mm}");

            if (tarde != null)
                partes.Add($"Tarde: {tarde.HoraInicio:HH\\:mm}-{tarde.HoraFim:HH\\:mm}");

            return partes.Count == 0 ? "-" : string.Join(" | ", partes);
        }
    }
}
