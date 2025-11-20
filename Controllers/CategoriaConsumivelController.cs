using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;
using HealthWellbeing.Data;

namespace HealthWellbeing.Controllers
{
    public class CategoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CategoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaConsumivel
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 10;

            // Inserção automática de categorias se estiver vazio
            if (!_context.CategoriaConsumivel.Any())
            {
                var categorias = new List<CategoriaConsumivel>
                {
                    new CategoriaConsumivel { Nome = "Luvas", Descricao = "Luvas descartáveis e estéreis para uso médico e cirúrgico." },
                    new CategoriaConsumivel { Nome = "Máscaras", Descricao = "Máscaras cirúrgicas, N95 e outros tipos de proteção respiratória." },
                    new CategoriaConsumivel { Nome = "Seringas e Agulhas", Descricao = "Seringas descartáveis, agulhas e acessórios de injeção." },
                    new CategoriaConsumivel { Nome = "Cateteres", Descricao = "Cateteres intravenosos, urinários e outros tipos médicos." },
                    new CategoriaConsumivel { Nome = "Compressas", Descricao = "Compressas estéreis e não estéreis para curativos e cirurgias." },
                    new CategoriaConsumivel { Nome = "Gazes", Descricao = "Gazes hospitalares e curativos de gaze esterilizados." },
                    new CategoriaConsumivel { Nome = "Bandagens", Descricao = "Faixas e ligaduras elásticas ou de imobilização." },
                    new CategoriaConsumivel { Nome = "Adesivos Médicos", Descricao = "Fitas adesivas, micropores e esparadrapos." },
                    new CategoriaConsumivel { Nome = "Soluções de Soro", Descricao = "Soro fisiológico, glicosado e soluções intravenosas." },
                    new CategoriaConsumivel { Nome = "Desinfetantes", Descricao = "Álcool, cloro, iodopovidona e outros agentes antissépticos." },
                    new CategoriaConsumivel { Nome = "Material de Aspiração", Descricao = "Tubos e frascos para aspiração de secreções." },
                    new CategoriaConsumivel { Nome = "Material de Oxigenoterapia", Descricao = "Cânulas, máscaras de oxigénio e tubos de ligação." },
                    new CategoriaConsumivel { Nome = "Equipamento de Infusão", Descricao = "Equipos de soro, extensões e conectores." },
                    new CategoriaConsumivel { Nome = "Material de Curativo", Descricao = "Kits de curativo e material para trocas de pensos." },
                    new CategoriaConsumivel { Nome = "Material de Punção", Descricao = "Agulhas, scalps e dispositivos de punção venosa." },
                    new CategoriaConsumivel { Nome = "Lâminas e Bisturis", Descricao = "Lâminas cirúrgicas e bisturis descartáveis." },
                    new CategoriaConsumivel { Nome = "Campos Cirúrgicos", Descricao = "Campos estéreis para cobertura de áreas cirúrgicas." },
                    new CategoriaConsumivel { Nome = "Toucas e Protetores", Descricao = "Toucas, propés e aventais descartáveis." },
                    new CategoriaConsumivel { Nome = "Material de Esterilização", Descricao = "Indicadores químicos, embalagens e fitas para esterilização." },
                    new CategoriaConsumivel { Nome = "Frascos e Recipientes", Descricao = "Frascos coletores e contentores para amostras biológicas." },
                    new CategoriaConsumivel { Nome = "Material de Coleta", Descricao = "Tubos de ensaio, agulhas de coleta e lancetas." },
                    new CategoriaConsumivel { Nome = "Equipamentos de Proteção Individual", Descricao = "EPI hospitalar como óculos, viseiras e aventais." },
                    new CategoriaConsumivel { Nome = "Material de Endoscopia", Descricao = "Acessórios descartáveis usados em procedimentos endoscópicos." },
                    new CategoriaConsumivel { Nome = "Material de Radiologia", Descricao = "Aventais de chumbo, protetores e filmes radiográficos." },
                    new CategoriaConsumivel { Nome = "Material de Laboratório", Descricao = "Pipetas, ponteiras, tubos e outros consumíveis laboratoriais." },
                    new CategoriaConsumivel { Nome = "Suturas", Descricao = "Fios de sutura absorvíveis e não absorvíveis." },
                    new CategoriaConsumivel { Nome = "Material de Hemoterapia", Descricao = "Bolsas de sangue, filtros e conjuntos de transfusão." },
                    new CategoriaConsumivel { Nome = "Material Odontológico", Descricao = "Consumíveis para uso em clínicas odontológicas hospitalares." },
                    new CategoriaConsumivel { Nome = "Material de Oftalmologia", Descricao = "Lentes, campos e instrumentos descartáveis para cirurgias oculares." },
                    new CategoriaConsumivel { Nome = "Material de Ortopedia", Descricao = "Gessos, talas e acessórios ortopédicos descartáveis." },
                    new CategoriaConsumivel { Nome = "Material de Ginecologia", Descricao = "Espéculos, sondas e kits ginecológicos descartáveis." },
                    new CategoriaConsumivel { Nome = "Material Pediátrico", Descricao = "Consumíveis hospitalares adaptados ao público infantil." },
                    new CategoriaConsumivel { Nome = "Material de Nutrição Enteral", Descricao = "Sondas e extensões para nutrição enteral." },
                    new CategoriaConsumivel { Nome = "Material de Diálise", Descricao = "Filtros, linhas e acessórios descartáveis para hemodiálise." },
                    new CategoriaConsumivel { Nome = "Material de Urologia", Descricao = "Sondas, bolsas coletoras e acessórios urológicos." },
                    new CategoriaConsumivel { Nome = "Material de Anestesia", Descricao = "Máscaras, circuitos e filtros para anestesia." },
                    new CategoriaConsumivel { Nome = "Material de Emergência", Descricao = "Kits de emergência, cânulas e acessórios para suporte básico de vida." },
                    new CategoriaConsumivel { Nome = "Material de Diagnóstico", Descricao = "Testes rápidos, tiras reagentes e materiais de diagnóstico in vitro." },
                    new CategoriaConsumivel { Nome = "Material de Higiene Hospitalar", Descricao = "Toalhetes, papel, sabão e produtos de limpeza hospitalar." },
                    new CategoriaConsumivel { Nome = "Material Diverso", Descricao = "Outros consumíveis de uso geral em ambiente hospitalar." }
                };
                _context.CategoriaConsumivel.AddRange(categorias);
                await _context.SaveChangesAsync();
            }

            // Consulta base
            var categoriasQuery = _context.CategoriaConsumivel.AsQueryable();

            // Aplicar filtro de pesquisa se houver
            if (!string.IsNullOrEmpty(searchString))
            {
                categoriasQuery = categoriasQuery.Where(c => c.Nome.Contains(searchString));
            }

            var totalCategorias = await categoriasQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategorias / (double)pageSize);

            var categoriasPagina = await categoriasQuery
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchString = searchString;

            return View(categoriasPagina);
        }

        // GET: CategoriaConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaConsumivel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaConsumivel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Categoria criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (id != categoriaConsumivel.CategoriaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaConsumivelExists(categoriaConsumivel.CategoriaId)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = "Categoria alterada com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null) return NotFound();

            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);

            if (categoriaConsumivel == null)
            {
                TempData["ErrorMessage"] = "A categoria selecionada não foi encontrada ou já foi eliminada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.CategoriaConsumivel.Remove(categoriaConsumivel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"A categoria \"{categoriaConsumivel.Nome}\" foi eliminada com sucesso.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"Não foi possível eliminar a categoria \"{categoriaConsumivel.Nome}\" porque está associada a outros registos.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado ao eliminar a categoria.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaConsumivelExists(int id)
        {
            return _context.CategoriaConsumivel.Any(e => e.CategoriaId == id);
        }
    }
}