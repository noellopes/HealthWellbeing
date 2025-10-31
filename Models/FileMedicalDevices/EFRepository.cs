/*
using HealthWellbeing.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeingRoom.Models.FileMedicalDevices
{
    public class EFRepository : InterfaceRepositoryMDev
    {
        private HealthWellbeingDbContext _dbcontext;
        public EFRepository(HealthWellbeingDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IEnumerable<MedicalDevices> devices => _dbcontext.MedicalDevices;

        public void AddMedicalDevices(MedicalDevices device) // Equivalente ao AddMedicalDevices
        {
            // 1. Adiciona o objeto ao contexto do EF Core (rastreamento de mudanças)
            _dbcontext.MedicalDevices.Add(device);

            // 2. SALVA A MUDANÇA NO BANCO DE DADOS
            _dbcontext.SaveChanges();
        }

        // Nota: O método Update é a forma mais limpa de fazer o EF Core rastrear mudanças.
        public void UpdateMedicalDevices(MedicalDevices updatedDevice) // Equivalente ao UpdateMedicalDevices
        {
            // 1. Diz ao EF Core para começar a rastrear o objeto modificado.
            // O EF Core irá gerar o SQL UPDATE comparando o objeto atual com o original, se estiver rastreado.
            _dbcontext.MedicalDevices.Update(updatedDevice);

            // 2. SALVA A MUDANÇA NO BANCO DE DADOS
            _dbcontext.SaveChanges();
        }

        public void DeleteMedicalDevices(int id) // Equivalente ao DeleteMedicalDevices
        {
            // 1. Encontra o dispositivo. Find é a forma mais rápida de buscar pela chave primária (DevicesID).
            var device = _dbcontext.MedicalDevices.Find(id);

            // 2. Se for encontrado, remove-o do DbSet
            if (device != null)
            {
                _dbcontext.MedicalDevices.Remove(device);

                // 3. SALVA A MUDANÇA NO BANCO DE DADOS
                _dbcontext.SaveChanges();
            }
        }
    }
}
*/
