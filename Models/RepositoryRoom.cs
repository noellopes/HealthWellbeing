using HealthWellbeing.Models;
using System.Linq;
using System.Collections.Generic;

namespace HealthWellbeingRoom.Models
{
    // Repositório estático para gerir objetos do tipo Room em memória
    public class RepositoryRoom
    {
        // Lista estática que armazena todas as salas
        private static List<Room> rooms = new List<Room>();

        // Propriedade pública para aceder à lista de salas (somente leitura)
        public static IEnumerable<Room> Rooms => rooms;

        // Método para adicionar uma nova sala à lista
        public static void AddRoom(Room room)
        {
            // Verifica se já existe uma sala com o mesmo ID antes de adicionar
            if (!rooms.Any(r => r.RoomId == room.RoomId))
            {
                rooms.Add(room);
            }
        }

        // Método para atualizar os dados de uma sala existente
        public static void UpdateRoom(Room updatedRoom)
        {
            // Procura a sala com o mesmo ID na lista
            var existingRoom = rooms.FirstOrDefault(r => r.RoomId == updatedRoom.RoomId);
            if (existingRoom != null)
            {
                // Atualiza os campos da sala com os novos valores
                existingRoom.Name = updatedRoom.Name;
                existingRoom.Capacity = updatedRoom.Capacity;
                existingRoom.Specialty = updatedRoom.Specialty;
                existingRoom.Location = updatedRoom.Location;
                existingRoom.Notes = updatedRoom.Notes;
                existingRoom.Status = updatedRoom.Status;
                existingRoom.RoomsType = updatedRoom.RoomsType;
                existingRoom.OperatingHours = updatedRoom.OperatingHours;
            }
        }

        // Método para remover uma sala da lista com base no ID
        public static void RemoveRoom(int id)
        {
            // Procura a sala com o ID especificado
            var roomToRemove = rooms.FirstOrDefault(r => r.RoomId == id);
            if (roomToRemove != null)
            {
                rooms.Remove(roomToRemove);
            }
        }

        // Método para obter uma sala específica pelo ID
        public static Room GetRoomById(int id)
        {
            return rooms.FirstOrDefault(r => r.RoomId == id);
        }
    }
}