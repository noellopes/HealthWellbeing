using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class RepositoryRoom
    {
        //lista estatica das salas
        public static List<Room> rooms = new List<Room>();

        //propriedade estatica para aceder a lista de salas
        public static IEnumerable<Room> Rooms => rooms;

        //metodo estatico para adicionar uma sala
        public static void AddRoom(Room room) => rooms.Add(room);
    }
}
