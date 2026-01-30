using HealthWellbeing.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Services
{
    public class ClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AtualizarNivelClienteAsync(int clienteId)
        {
            var cliente = await _context.ClientesBalneario
                .Include(c => c.HistoricoPontos)
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == clienteId);

            if (cliente == null)
                return;

            int totalPontos = cliente.HistoricoPontos.Sum(p => p.Pontos);

            var nivel = await _context.NiveisCliente
                .Where(n => totalPontos >= n.PontosMinimos)
                .OrderByDescending(n => n.PontosMinimos)
                .FirstOrDefaultAsync();

            cliente.NivelClienteId = nivel?.NivelClienteId;

            await _context.SaveChangesAsync();
        }
    }
}
