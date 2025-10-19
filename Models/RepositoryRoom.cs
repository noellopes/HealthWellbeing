using HealthWellbeing.Models;
using System.Linq;

namespace HealthWellbeingRoom.Models
{
    public class RepositoryRoom
    {
        // Lista estática das salas
        private static List<Room> rooms = new List<Room>();

        // Propriedade estática para aceder à lista de salas
        public static IEnumerable<Room> Rooms => rooms;

        // Método estático para adicionar uma sala
        public static void AddRoom(Room room)
        {
            // Garante que o ID é único
            if (!rooms.Any(r => r.SalaId == room.SalaId))
            {
                rooms.Add(room);
            }
        }

        // Método estático para atualizar uma sala existente
        public static void UpdateRoom(Room updatedRoom)
        {
            var salaExistente = rooms.FirstOrDefault(r => r.SalaId == updatedRoom.SalaId);
            if (salaExistente != null)
            {
                salaExistente.Nome = updatedRoom.Nome;
                salaExistente.Capacidade = updatedRoom.Capacidade;
                salaExistente.Especialidade = updatedRoom.Especialidade;
                salaExistente.Localizacao = updatedRoom.Localizacao;
                salaExistente.Observacoes = updatedRoom.Observacoes;
                salaExistente.Disponibilidade = updatedRoom.Disponibilidade;
            }
        }

        // Método estático para remover uma sala
        public static void RemoveRoom(int id)
        {
            var roomToRemove = rooms.FirstOrDefault(r => r.SalaId == id);
            if (roomToRemove != null)
            {
                rooms.Remove(roomToRemove);
            }
        }

        // Método estático para obter uma sala por ID
        public static Room GetRoomById(int id)
        {
            return rooms.FirstOrDefault(r => r.SalaId == id);
        }
    }
}