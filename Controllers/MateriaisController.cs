using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para o SelectListItem do dropdown

namespace HealthWellbeing.Controllers
{
    // O nome do controlador segue o padrão [Modelo]Controller
    public class MaterialEquipamentoAssociadoController : Controller
    {
        // GET: /MaterialEquipamentoAssociado/VerEquipamentos
        // (Seguindo o padrão VerProfissionais)
        public IActionResult VerEquipamentos()
        {
            // Para testar dados exemplo
            var equipamentos = new List<MaterialEquipamentoAssociado>
            {
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 1,
                    NomeEquipamento = "Passadeira Elétrica XPTO",
                    Quantidade = 2,
                    EstadoComponente = "Operacional"
                },
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 2,
                    NomeEquipamento = "Conjunto Halteres (5kg-20kg)",
                    Quantidade = 5,
                    EstadoComponente = "Operacional"
                },
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 3,
                    NomeEquipamento = "Bicicleta Estática B100",
                    Quantidade = 1,
                    EstadoComponente = "Em manutenção"
                },
                new MaterialEquipamentoAssociado
                {
                    MaterialEquipamentoAssociadoId = 4,
                    NomeEquipamento = "Máquina Elíptica E50",
                    Quantidade = 1,
                    EstadoComponente = "Avariado"
                }
            };

            // Passa a lista de equipamentos para a View
            return View(equipamentos);
        }

        // GET: /MaterialEquipamentoAssociado/CriarEquipamento
        // (Seguindo o padrão CriarProfissional)
        [HttpGet]
        public IActionResult CriarEquipamento()
        {
            // Similar ao exemplo das Funcoes, vamos preparar os Estados
            // para um dropdown, visto que o campo EstadoComponente sugere
            // valores pré-definidos.
            ViewBag.EstadosDisponiveis = new List<SelectListItem>
            {
                new SelectListItem { Value = "Operacional", Text = "Operacional" },
                new SelectListItem { Value = "Em manutenção", Text = "Em manutenção" },
                new SelectListItem { Value = "Avariado", Text = "Avariado" }
            };

            return View();
        }

        // POST: /MaterialEquipamentoAssociado/CriarEquipamento
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção anti-CSRF
        public IActionResult CriarEquipamento(MaterialEquipamentoAssociado equipamento)
        {
            // Verifica se o modelo recebido é válido (baseado nos Data Annotations)
            if (ModelState.IsValid)
            {
                // Lógica para guardar o novo Equipamento na base de dados (EF Core, etc.)
                // ... (simulação)

                // Mensagem de sucesso para o utilizador
                TempData["SuccessMessage"] = $"Equipamento '{equipamento.NomeEquipamento}' criado com sucesso!";

                // Redireciona para a lista de equipamentos
                return RedirectToAction("VerEquipamentos");
            }

            // Se o modelo não for válido, retorna à View de criação
            // É crucial recarregar o ViewBag/ViewData para o dropdown
            ViewBag.EstadosDisponiveis = new List<SelectListItem>
            {
                new SelectListItem { Value = "Operacional", Text = "Operacional" },
                new SelectListItem { Value = "Em manutenção", Text = "Em manutenção" },
                new SelectListItem { Value = "Avariado", Text = "Avariado" }
            };

            return View(equipamento);
        }

        // adicionar outras ações como Editar, Detalhes e Apagar aqui.
    }
}