using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Services
{
    public class UtenteService
    {
        private readonly ApplicationDbContext _context;

        public UtenteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UtenteBalneario> GetAll()
        {
            return _context.Utentes
                .Include(u => u.DadosMedicos)
                .Include(u => u.SeguroSaude)
                .OrderBy(u => u.NomeCompleto)
                .ToList();
        }

        public UtenteBalneario? GetById(int id)
        {
            return _context.Utentes
                .Include(u => u.DadosMedicos)
                .Include(u => u.SeguroSaude)
                .FirstOrDefault(u => u.UtenteBalnearioId == id);
        }

        public void Add(UtenteBalneario utente)
        {
            _context.Utentes.Add(utente);
            _context.SaveChanges();
        }

        public void Update(UtenteBalneario utente)
        {
            _context.Utentes.Update(utente);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var utente = _context.Utentes.Find(id);
            if (utente == null) return;

            _context.Utentes.Remove(utente);
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Utentes.Count();
        }

        public List<UtenteBalneario> GetPage(int page, int pageSize)
        {
            return _context.Utentes
                .Include(u => u.DadosMedicos)
                .Include(u => u.SeguroSaude)
                .OrderBy(u => u.NomeCompleto)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

    }
}
