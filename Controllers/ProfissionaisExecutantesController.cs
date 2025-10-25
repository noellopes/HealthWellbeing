using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário se for usar SelectListItem para dropdowns

namespace HealthWellbeing.Controllers
{
    // O nome do controlador deve corresponder ao modelo, terminando em "Controller"
    public class ProfissionalExecutanteController : Controller
    {
        // GET: /ProfissionalExecutante/VerProfissionais
        public IActionResult VerProfissionais()
        {
            // Para testar dados exemplo
            var profissionais = new List<ProfissionalExecutante>
            {
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 1,
                    Nome = "Dr. João Silva",
                    Funcao = "Fisioterapeuta",
                    Telefone = "912345678",
                    Email = "joao.silva@clinica.pt"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 2,
                    Nome = "Dra. Maria Santos",
                    Funcao = "Nutricionista",
                    Telefone = "967890123",
                    Email = "maria.santos@saude.pt"
                },
                new ProfissionalExecutante
                {
                    ProfissionalExecutanteId = 3,
                    Nome = "Prof. Carlos Lima",
                    Funcao = "Personal Trainer",
                    Telefone = "934567890",
                    Email = "carlos.lima@fit.com"
                }
            };

            // Passa a lista de profissionais para a View
            return View(profissionais);
        }

        // GET: /ProfissionalExecutante/CriarProfissional
        [HttpGet]
        public IActionResult CriarProfissional()
        {
            // Se a Funcao fosse uma entidade separada 

            /*
            ViewBag.FuncoesDisponiveis = new List<SelectListItem>
            {
                 new SelectListItem { Value = "Fisio", Text = "Fisioterapeuta" },
                 new SelectListItem { Value = "Nutri", Text = "Nutricionista" }
            };
            */

            return View();
        }

        // POST: /ProfissionalExecutante/CriarProfissional
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção anti-CSRF
        public IActionResult CriarProfissional(ProfissionalExecutante profissional)
        {
            // Verifica se o modelo recebido é válido de acordo com os Data Annotations
            if (ModelState.IsValid)
            {
                // Lógica para guardar o novo Profissional na base de dados (EF Core, etc.)


              
                TempData["SuccessMessage"] = $"Profissional '{profissional.Nome}' criado com sucesso!";
               
                return RedirectToAction("VerProfissionais");
            }

            // Se foi carregado ViewBag.FuncoesDisponiveis no GET, precisaria recarregá-las aqui também

            return View(profissional);
        }

        // adicionar outras ações como Editar, Detalhes e Apagar aqui.
    }
}