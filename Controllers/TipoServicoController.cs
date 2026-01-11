using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;


namespace HealthWellbeing.Controllers
{
    public class TipoServicoController : Controller
    {
        
        private readonly HealthWellbeingDbContext _context;

        public TipoServicoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TipoServico
        public async Task<IActionResult> Index(string pesquisarNome, int pagina = 1)
        {
            
            var consulta = _context.TipoServicos.AsQueryable();

            if (!string.IsNullOrEmpty(pesquisarNome))
            {
                consulta = consulta.Where(x => x.Nome.Contains(pesquisarNome));
            }


            int pageSize = 6;
            int totalRegistos = await consulta.CountAsync();

            var model = new TipoServicoViewModel
            {
                PesquisarNome = pesquisarNome,
                paginacao = new Paginacao(totalRegistos, pagina, pageSize),
                ListaServicos = await consulta
                    .OrderBy(x => x.Nome)
                    .Skip((pagina - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };


            return View(model);
        }



        // GET: TipoServico/Details/5
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Details(int? id, string? successMessage = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServico = await _context.TipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicosId == id);

            if (tipoServico == null)
            {
                return NotFound();
            }

            ViewBag.SuccessMessage = successMessage;
            return View(tipoServico);
        }

        // GET: TipoServico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoServico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoServicoId,Nome,Descricao")] TipoServicos tipoServico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoServico);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details),
                    new { id = tipoServico.TipoServicosId, successMessage = "Tipo de Serviço criado com sucesso!" });
            }
            return View(tipoServico);
        }

        // GET: TipoServico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoServico = await _context.TipoServicos.FindAsync(id);
            if (tipoServico == null)
            {
                return NotFound();
            }
            return View(tipoServico);
        }
        // POST: TipoServico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoServicosId,Nome,Descricao")] TipoServicos tipoServico)
        {
            if (id != tipoServico.TipoServicosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoServico);
                    await _context.SaveChangesAsync();

                    
                    return RedirectToAction(nameof(Details), new
                    {
                        id = tipoServico.TipoServicosId,
                        successMessage = "Dados do Tipo de Serviço alterado com sucesso!"
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoServicoExists(tipoServico.TipoServicosId))
                    {
                        return NotFound();
                    }
                    else { throw; }
                }
            }
            return View(tipoServico);
        }

        // GET: TipoServico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tipoServico = await _context.TipoServicos
                .FirstOrDefaultAsync(m => m.TipoServicosId == id);

            if (tipoServico == null)
                return NotFound();

            return View(tipoServico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tipoServico = await _context.TipoServicos.FindAsync(id);

            if (tipoServico != null)
            {
                _context.TipoServicos.Remove(tipoServico);
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Index), new { successMessage = "Tipo de Serviço eliminado com sucesso!" });
        }

        private bool TipoServicoExists(int id)
        {
            return _context.TipoServicos.Any(e => e.TipoServicosId == id);
        }
        
        }
    }