using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class MemberController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- AÇÃO INDEX ATUALIZADA ---
        // GET: Member
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchPhone = "", string searchEmail = "")
        {
            // Criar a query base, incluindo o Cliente
            var membersQuery = _context.Member
                .Include(m => m.Client)
                .AsQueryable();

            // Adicionar lógica de pesquisa (através do Cliente)
            // Adicionámos "m.Client != null" para segurança
            if (!string.IsNullOrEmpty(searchName))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Name.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Phone.Contains(searchPhone));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                membersQuery = membersQuery.Where(m => m.Client != null && m.Client.Email.Contains(searchEmail));
            }

            // Passar a pesquisa para o ViewBag
            ViewBag.SearchName = searchName;
            ViewBag.SearchPhone = searchPhone;
            ViewBag.SearchEmail = searchEmail;

            // Adicionar lógica de paginação
            int numberMembers = await membersQuery.CountAsync();
            // Quantos itens por página quiser
            var membersInfo = new PaginationInfo<Member>(page, numberMembers, 5);

            membersInfo.Items = await membersQuery
                .OrderBy(m => m.Client.Name) // Ordenar pelo nome do cliente
                .Skip(membersInfo.ItemsToSkip)
                .Take(membersInfo.ItemsPerPage)
                .ToListAsync();

            return View(membersInfo);
        }

        // --- FIM DA AÇÃO INDEX ATUALIZADA ---


        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member
                .Include(m => m.Client) // Include the Client data
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // --- CREATE ACTIONS ---
        public IActionResult Create(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                TempData["Message"] = "Client ID is required to create a membership.";
                return RedirectToAction("Index", "Client");
            }
            var client = _context.Client.Find(clientId);
            if (client == null)
            {
                TempData["Message"] = $"Client ID '{clientId}' not found.";
                return RedirectToAction("Index", "Client");
            }
            if (_context.Member.Any(m => m.ClientId == clientId))
            {
                TempData["Message"] = $"Client <strong>{client.Name}</strong> is already an active member.";
                TempData["MessageType"] = "warning";
                return RedirectToAction("Index", "Client");
            }

            var member = new Member { ClientId = clientId };
            // Passar todos os detalhes para a View de confirmação
            ViewBag.ClientName = client.Name;
            ViewBag.ClientPhone = client.Phone;
            ViewBag.ClientEmail = client.Email;

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId")] Member member)
        {
            if (string.IsNullOrEmpty(member.ClientId))
            {
                ModelState.AddModelError("ClientId", "Client ID is missing.");
            }

            var client = await _context.Client.FindAsync(member.ClientId);

            if (ModelState.IsValid && client != null)
            {
                member.Client = client;
                _context.Add(member);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Membership successfully created for Client: <strong>{client.Name}</strong>";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "Client");
            }

            // Se o ModelState falhar, repopular o ViewBag
            ViewBag.ClientName = client?.Name ?? "Unknown Client";
            ViewBag.ClientPhone = client?.Phone ?? "N/A";
            ViewBag.ClientEmail = client?.Email ?? "N/A";

            return View(member);
        }

        // GET: Member/Edit/5 (O 'id' aqui é o MemberId)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // 1. Encontrar o Membro E incluir o Cliente associado
            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null || member.Client == null)
            {
                return NotFound();
            }

            // 2. Enviar o OBJETO CLIENTE (e não o Membro) para a View
            return View(member.Client);
        }

        // POST: Member/Edit/5
        // A View vai enviar os dados de um Client, por isso o [Bind] é para o Client
        [HttpPost]
        [ValidateAntiForgeryToken]
        // O 'id' (MemberId) é ignorado, e o 'ClientId' vem do form
        public async Task<IActionResult> Edit(string ClientId, [Bind("ClientId,Name,Email,Phone,Address,BirthDate,Gender")] Client client)
        {
            // 1. Validar que o ClientId do form corresponde ao objeto
            if (ClientId != client.ClientId)
            {
                return NotFound();
            }

            // 2. Validar o modelo do CLIENTE
            if (ModelState.IsValid)
            {
                try
                {
                    // 3. Atualizar o CLIENTE na base de dados
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Client.Any(e => e.ClientId == client.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // 4. Redirecionar de volta para o ÍNDICE DE MEMBROS
                TempData["Message"] = $"Dados do cliente <strong>{client.Name}</strong> atualizados com sucesso.";
                TempData["MessageType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            // Se o modelo falhar, volta à View de Edição
            return View(client);
        }

        // GET: Member/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            // É importante incluir o Cliente aqui para a View de Delete
            var member = await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Member.FindAsync(id);
            if (member != null)
            {
                _context.Member.Remove(member);
            }
            await _context.SaveChangesAsync();

            // Mensagem de sucesso para a página de Membros
            TempData["Message"] = "Membership successfully deleted.";
            TempData["MessageType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Member.Any(e => e.MemberId == id);
        }
    }
}