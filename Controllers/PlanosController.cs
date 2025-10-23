using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;


namespace HealthWellbeing.Controllers
{
    public class PlanoController : Controller
    {
        //Lista de planos 
        private static readonly List<Plano> _planos = new()
        {
            new Plano
            {
                PlanoId = 1,
                Nome = "Mensal",
                Descricao = "Acesso total ao ginásio por 30 dias.",
                Preco = 29.99m,
                DuracaoDias = 30
            },
            new Plano
            {
                PlanoId = 2,
                Nome = "Semestral",
                Descricao = "Acesso total por 6 meses com desconto de 15% em relação ao plano mensal.",
                Preco = 149.99m,
                DuracaoDias = 180
            },
            new Plano
            {
                PlanoId = 3,
                Nome = "Anual",
                Descricao = "Plano completo de 12 meses com o melhor custo-benefício. Inclui avaliação física grátis.",
                Preco = 249.99m,
                DuracaoDias = 365
            }
        };

        // GET: PlanoController
        public IActionResult Index()
        {
            return View(_planos);
        }
    }
}


