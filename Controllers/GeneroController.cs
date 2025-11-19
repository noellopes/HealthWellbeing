using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class GeneroController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public GeneroController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Generos
        public async Task<IActionResult> Index(string searchNomeGenero, int page = 1)
        {
            // Consulta base
            var generosQuery = _context.Genero.AsQueryable();

            // Lógica de Filtro (Pesquisa)
            if (!string.IsNullOrEmpty(searchNomeGenero))
            {
                // Filtra pelo nome do género
                generosQuery = generosQuery
                    .Where(g => g.NomeGenero.Contains(searchNomeGenero));

                // Guarda o texto na ViewBag para manter na caixa de pesquisa após o reload
                ViewBag.SearchNomeGenero = searchNomeGenero;
            }

            // Contar total de itens (já considerando o filtro, se existir)
            int totalGeneros = await generosQuery.CountAsync();

            // Criar objeto de paginação
            var generosInfo = new PaginationInfo<Genero>(page, totalGeneros);

            // Buscar os itens da página atual
            generosInfo.Items = await generosQuery
                .OrderBy(g => g.NomeGenero)
                .Skip(generosInfo.ItemsToSkip)
                .Take(generosInfo.ItemsPerPage)
                .ToListAsync();

            return View(generosInfo);
        }


        // GET: Genero/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Genero
                .FirstOrDefaultAsync(m => m.GeneroId == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Genero/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genero/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GeneroId,NomeGenero")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = genero.GeneroId,
                        SuccessMessage = "Genero criado com sucesso"
                    }
                );
            }
            return View(genero);
        }

        // GET: Genero/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Genero.FindAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Genero/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GeneroId,NomeGenero")] Genero genero)
        {
            if (id != genero.GeneroId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o registo na BD para garantir que existe
                    var generoExistente = await _context.Genero.FindAsync(id);

                    // SE O GÉNERO FOI APAGADO (é null), mostramos a página de recuperação
                    if (generoExistente == null)
                    {
                        return View("InvalidGenero", genero);
                    }

                    // Atualiza os valores do registo existente com os novos valores do formulário
                    _context.Entry(generoExistente).CurrentValues.SetValues(genero);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),
                        new
                        {
                            id = genero.GeneroId,
                            SuccessMessage = "Genero editado com sucesso"
                        });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.GeneroId))
                    {
                        // Caso a exceção ocorra exatamente no momento do save
                        return View("GeneroDeleted", genero);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(genero);
        }

        // GET: Genero/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Carregar o género INCLUINDO a lista de exercícios associados
            var genero = await _context.Genero
                .Include(g => g.ExercicioGeneros) // Importante: Carregar a relação
                .FirstOrDefaultAsync(m => m.GeneroId == id);

            if (genero == null)
            {
                TempData["SuccessMessage"] = "Este gênero já foi eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Verificar quantos exercícios usam este género
            int numExercicios = genero.ExercicioGeneros.Count;

            // 3. Passar essa informação para a View
            ViewBag.NumExercicios = numExercicios;
            ViewBag.PodeEliminar = numExercicios == 0;

            return View(genero);
        }

        // POST: Genero/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1. Carregar novamente com a relação para garantir segurança no Back-end
            var genero = await _context.Genero
                .Include(g => g.ExercicioGeneros)
                .FirstOrDefaultAsync(m => m.GeneroId == id);

            if (genero != null)
            {
                // 2. Verificação de Segurança Final
                if (genero.ExercicioGeneros.Any())
                {
                    // Se alguém tentar forçar a eliminação via código malicioso ou URL
                    TempData["ErrorMessage"] = "Não é possível eliminar este gênero porque existem exercícios associados.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Genero.Remove(genero);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Gênero foi apagado com sucesso.";
            }
            else
            {
                // Caso o género já não exista
                TempData["SuccessMessage"] = "Este gênero já tinha sido eliminado.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
            return _context.Genero.Any(e => e.GeneroId == id);
        }
    }
}
