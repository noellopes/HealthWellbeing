using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

// 1. CORRIGIDO: Usa o namespace correto para o DbContext
using HealthWellbeing.Data;
// 2. Onde estão os modelos
using HealthWellBeing.Models;

namespace HealthWellBeing.Controllers
{
    public class ExamesController : Controller
    {
        // Injeção de Dependência do DbContext
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exames (Lista todas as marcações de exame)
        public async Task<IActionResult> Index()
        {
            // Usa .Include() para carregar dados relacionados (como o professor fez)
            var exames = _context.Exames
                .Include(e => e.Utente) // Utente (Paciente)
                .Include(e => e.ExameTipo) // Tipo de Exame (TAC, Raio-X)
                .Include(e => e.MedicoSolicitante); // Médico Solicitante

            return View(await exames.ToListAsync());
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Carrega todas as relações para a vista de detalhes
            var exame = await _context.Exames
                .Include(e => e.Utente)
                .Include(e => e.ExameTipo)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.MaterialEquipamentoAssociado)
                .FirstOrDefaultAsync(m => m.ExameId == id); // Usa ExameId para a chave

            if (exame == null) return NotFound();

            return View(exame);
        }

        // GET: Exames/Create (Prepara o formulário de marcação)
        public IActionResult Create()
        {
            // Cria SelectLists para os Dropdowns
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "NomeCompleto");
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipos, "ExameTipoId", "Nome");
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "Nome");

            // Outras relações que podem ser escolhidas no formulário
            ViewData["SalaId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoDeSala");
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionaisExecutantes, "ProfissionalExecutanteId", "Nome");
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento");

            return View();
        }

        // POST: Exames/Create (Recebe os dados da marcação)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Bind()] é crucial para segurança
        public async Task<IActionResult> Create(
            [Bind("ExameId,DataHoraMarcacao,Notas,UtenteId,ExameTipoId,MedicoId,SalaId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                // Define o estado inicial como Marcado
                exame.Estado = EstadoExame.Marcado;

                _context.Add(exame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarrega as SelectLists em caso de erro de validação
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "NomeCompleto", exame.UtenteId);
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipos, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "Nome", exame.MedicoId);
            ViewData["SalaId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoDeSala", exame.SalaDeExameId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionaisExecutantes, "ProfissionalExecutanteId", "Nome", exame.ProfissionalExecutanteId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento", exame.MaterialEquipamentoAssociadoId);


            return View(exame);
        }

        // GET: Exames/Edit/5 (Similar ao UtentesController/BooksController)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();

            // Recarrega as SelectLists para a View
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "NomeCompleto", exame.UtenteId);
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipos, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "Nome", exame.MedicoId);
            ViewData["SalaId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoDeSala", exame.SalaDeExameId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionaisExecutantes, "ProfissionalExecutanteId", "Nome", exame.ProfissionalExecutanteId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento", exame.MaterialEquipamentoAssociadoId);


            return View(exame);
        }

        // POST: Exames/Edit/5 (Atualiza os dados da marcação)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameId,DataHoraMarcacao,Notas,Estado,UtenteId,ExameTipoId,MedicoId,SalaId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (id != exame.ExameId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exames.Any(e => e.ExameId == exame.ExameId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Recarrega as SelectLists em caso de erro
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "NomeCompleto", exame.UtenteId);
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipos, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "MedicoId", "Nome", exame.MedicoId);
            ViewData["SalaId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoDeSala", exame.SalaDeExameId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionaisExecutantes, "ProfissionalExecutanteId", "Nome", exame.ProfissionalExecutanteId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento", exame.MaterialEquipamentoAssociadoId);

            return View(exame);
        }

        // GET: Exames/Cancel/5 (Método para alterar o estado para "Cancelado")
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            // Carrega os dados relacionados para a View de confirmação
            var exame = await _context.Exames
                .Include(e => e.Utente)
                .Include(e => e.ExameTipo)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame == null) return NotFound();

            return View(exame);
        }

        // POST: Exames/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var exame = await _context.Exames.FindAsync(id);
            if (exame != null)
            {
                // Altera o estado para Cancelado (melhor prática que apagar)
                exame.Estado = EstadoExame.Cancelado;
                _context.Update(exame);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
