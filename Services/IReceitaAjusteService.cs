using System.Threading.Tasks;

namespace HealthWellbeing.Services
{
    public interface IReceitaAjusteService
    {
        Task<PlanoReceitasAjustadas> GerarAjustesAsync(string clientId, int planoAlimentarId);
    }
}
