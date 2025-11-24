using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelFornecedorController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelFornecedorController(HealthWellbeingDbContext context)
        {
            _context = context;

            if (!_context.ConsumivelFornecedor.Any())
            {
                var fornecedores = new List<ConsumivelFornecedor>
                {
                    new ConsumivelFornecedor { NomeEmpresa = "MediHealth Portugal", NIF = "509823471", Morada = "Rua da Saúde, 45, Lisboa", Telefone = "912345678", Email = "contacto@medihealth.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "HospitalarPlus", NIF = "501239874", Morada = "Avenida dos Hospitais, 120, Porto", Telefone = "934567890", Email = "geral@hospitalarplus.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "BioClean Serviços Médicos", NIF = "507654321", Morada = "Rua das Clínicas, 18, Coimbra", Telefone = "917654321", Email = "info@bioclean.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "MedSupply Lda", NIF = "509111222", Morada = "Parque Industrial de Gaia, Armazém 3", Telefone = "969111222", Email = "vendas@medsupply.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "EquipHospi", NIF = "510333444", Morada = "Rua do Hospital, 9, Aveiro", Telefone = "915333444", Email = "suporte@equiphosp.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "TecnoMedica", NIF = "513987654", Morada = "Rua da Inovação Médica, 22, Braga", Telefone = "933987654", Email = "geral@tecnomedica.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "SterilCare", NIF = "507222333", Morada = "Zona Industrial Norte, Lote 10, Leiria", Telefone = "919222333", Email = "comercial@sterilcare.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "BioPharma PT", NIF = "505444555", Morada = "Rua dos Laboratórios, 6, Faro", Telefone = "962444555", Email = "info@biopharmapt.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "Soluções Hospitalares Lda", NIF = "508111999", Morada = "Avenida Europa, 87, Lisboa", Telefone = "968111999", Email = "contacto@solucoeshosp.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "Clínica Distribuição", NIF = "509888777", Morada = "Rua da Saúde Pública, 15, Santarém", Telefone = "912888777", Email = "geral@clinicadist.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "HospiEquipamentos Lda", NIF = "509222111", Morada = "Rua Central, 200, Viseu", Telefone = "931222111", Email = "vendas@hospiequip.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "MediStock Portugal", NIF = "509777333", Morada = "Rua dos Armazéns, 5, Évora", Telefone = "938777333", Email = "info@medistock.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "LabCare Distribuição", NIF = "502456789", Morada = "Rua dos Laboratórios, 9, Funchal", Telefone = "914456789", Email = "suporte@labcare.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "PharmaLine", NIF = "506333666", Morada = "Rua das Farmácias, 17, Setúbal", Telefone = "965333666", Email = "contato@pharmaline.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "SafeMed Portugal", NIF = "509555888", Morada = "Rua do Progresso, 13, Guarda", Telefone = "931555888", Email = "info@safemed.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "HospitalTech", NIF = "510666999", Morada = "Parque Empresarial de Braga, Lote 4", Telefone = "934666999", Email = "geral@hospitaltech.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "EcoMed Solutions", NIF = "507999000", Morada = "Rua Verde, 22, Castelo Branco", Telefone = "939999000", Email = "info@ecomed.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "VitalCare", NIF = "503444222", Morada = "Rua das Clínicas, 10, Beja", Telefone = "968444222", Email = "contacto@vitalcare.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "CleanHosp", NIF = "505777999", Morada = "Zona Industrial, Lote 8, Portalegre", Telefone = "911777999", Email = "vendas@cleanhosp.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "Distribuidora Médica Nacional", NIF = "501234567", Morada = "Rua do Comércio, 45, Lisboa", Telefone = "933234567", Email = "geral@dmn.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "Medicalis Lda", NIF = "509121212", Morada = "Rua da Medicina, 88, Braga", Telefone = "934121212", Email = "suporte@medicalis.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "InfusionMed", NIF = "504333111", Morada = "Rua do Hospital Universitário, Coimbra", Telefone = "938333111", Email = "contacto@infusionmed.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "HealthPlus", NIF = "502222333", Morada = "Rua Nova Saúde, 70, Lisboa", Telefone = "962222333", Email = "info@healthplus.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "ProMedCare", NIF = "506111222", Morada = "Rua Central Médica, 55, Porto", Telefone = "911111222", Email = "vendas@promedcare.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "InovaClinic", NIF = "509333444", Morada = "Rua da Inovação, 20, Aveiro", Telefone = "933333444", Email = "geral@inovaclinic.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "HospitalLog", NIF = "508555666", Morada = "Rua da Logística, 8, Faro", Telefone = "915555666", Email = "suporte@hospitallog.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "CareDistribuição", NIF = "509777888", Morada = "Rua dos Fornecedores, 31, Coimbra", Telefone = "936777888", Email = "info@caredist.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "SaniPortugal", NIF = "505999000", Morada = "Rua da Higiene, 19, Leiria", Telefone = "937999000", Email = "contato@sanipt.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "GlobalMed Lda", NIF = "501555777", Morada = "Rua Internacional, 44, Porto", Telefone = "961555777", Email = "vendas@globalmed.pt" },
                    new ConsumivelFornecedor { NomeEmpresa = "CliniPro", NIF = "506888999", Morada = "Rua da Profissionalização, 33, Lisboa", Telefone = "939888999", Email = "geral@clinipro.pt" }
                };

                _context.ConsumivelFornecedor.AddRange(fornecedores);
                _context.SaveChanges();
            }

        }

        // GET: ConsumivelFornecedor
        public async Task<IActionResult> Index(int page = 1, string searchNome = "")
        {

            var fornecedoresQuery = _context.ConsumivelFornecedor.AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                fornecedoresQuery = fornecedoresQuery
                    .Where(f => f.NomeEmpresa.Contains(searchNome));


            ViewBag.SearchNome = searchNome;

            int totalFornecedores = await fornecedoresQuery.CountAsync();

            var pagination = new PaginationInfo<ConsumivelFornecedor>(page, totalFornecedores);

            pagination.Items = await fornecedoresQuery
                .OrderBy(z => z.NomeEmpresa)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: ConsumivelFornecedor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivelFornecedor = await _context.ConsumivelFornecedor
                .FirstOrDefaultAsync(m => m.FornecedorId == id);
            if (consumivelFornecedor == null)
            {
                return NotFound();
            }

            return View(consumivelFornecedor);
        }

        // GET: ConsumivelFornecedor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ConsumivelFornecedor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FornecedorId,NomeEmpresa,NIF,Morada,Telefone,Email")] ConsumivelFornecedor consumivelFornecedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivelFornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consumivelFornecedor);
        }

        // GET: ConsumivelFornecedor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivelFornecedor = await _context.ConsumivelFornecedor.FindAsync(id);
            if (consumivelFornecedor == null)
            {
                return NotFound();
            }
            return View(consumivelFornecedor);
        }

        // POST: ConsumivelFornecedor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FornecedorId,NomeEmpresa,NIF,Morada,Telefone,Email")] ConsumivelFornecedor consumivelFornecedor)
        {
            if (id != consumivelFornecedor.FornecedorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivelFornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumivelFornecedorExists(consumivelFornecedor.FornecedorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(consumivelFornecedor);
        }

        // GET: ConsumivelFornecedor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivelFornecedor = await _context.ConsumivelFornecedor
                .FirstOrDefaultAsync(m => m.FornecedorId == id);
            if (consumivelFornecedor == null)
            {
                return NotFound();
            }

            return View(consumivelFornecedor);
        }

        // POST: ConsumivelFornecedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivelFornecedor = await _context.ConsumivelFornecedor.FindAsync(id);
            if (consumivelFornecedor != null)
            {
                _context.ConsumivelFornecedor.Remove(consumivelFornecedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumivelFornecedorExists(int id)
        {
            return _context.ConsumivelFornecedor.Any(e => e.FornecedorId == id);
        }
    }
}
