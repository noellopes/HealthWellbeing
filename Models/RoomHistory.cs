using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class RoomHistory
    {
        public int RoomHistoryId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //No futuro será uma entidade, Medico 
        public string Responsible { get; set; } = string.Empty;

        public int ResponsibleId { get; set; }

        public string Note { get; set; } = string.Empty;


        // FK para a sala
        public int RoomId { get; set; }
        public Room? Room { get; set; }


    }
}
