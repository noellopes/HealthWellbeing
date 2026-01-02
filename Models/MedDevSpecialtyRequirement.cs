using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Models
{
    public class MedDevSpecialtyRequirement
    {
        public int ID { get; set; }

        //Liga à tabela Specialty que criaste
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        //Nome do dispositivo (ex: "Monitor X")
        public string RequiredDeviceName { get; set; }

        //Quantidade necessaria
        public int Quantity { get; set; }

    }
}

