using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Controllers
{
    // Controlador responsável pela gestão das salas
    public class RoomsController : Controller
    {
        // Injeção do contexto da base de dados
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        // Lista todas as salas existentes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Room.ToListAsync());
        }

        // GET: Rooms/Details/5
        // Mostra os detalhes de uma sala específica
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Se o ID não for fornecido, retorna erro 404
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
            {
                return NotFound(); // Se a sala não existir, retorna erro 404
            }

            return View(room); // Renderiza a view com os dados da sala
        }

        // GET: Rooms/Create
        // Exibe o formulário para criar uma nova sala
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // Processa os dados enviados pelo formulário de criação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,RoomsType,Specialty,Name,Capacity,Location,OperatingHours,Status,Notes")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room); // Adiciona a nova sala ao contexto
                await _context.SaveChangesAsync(); // Salva na base de dados
                return RedirectToAction(nameof(Index)); // Redireciona para a lista
            }
            return View(room); // Se houver erro, retorna à view com os dados preenchidos
        }

        // GET: Rooms/Edit/5
        // Exibe o formulário para editar uma sala existente
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST: Rooms/Edit/5
        // Processa os dados enviados pelo formulário de edição
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,RoomsType,Specialty,Name,Capacity,Location,OperatingHours,Status,Notes")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound(); // Se o ID não corresponder, retorna erro
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room); // Atualiza os dados da sala
                    await _context.SaveChangesAsync(); // Salva na base de dados
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
                    {
                        return NotFound(); // Se a sala não existir, retorna erro
                    }
                    else
                    {
                        throw; // Lança exceção se for outro tipo de erro
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(room); // Se houver erro, retorna à view com os dados preenchidos
        }

        // GET: Rooms/Delete/5
        // Exibe a confirmação para apagar uma sala
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        // Processa a confirmação de exclusão
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room); // Remove a sala do contexto
            }

            await _context.SaveChangesAsync(); // Salva as alterações
            return RedirectToAction(nameof(Index));
        }

        // Verifica se uma sala existe com base no ID
        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}