using HealthWellbeing.Models;

namespace HealthWellbeing.ViewModels
{
    public class TypeMaterialDeleteViewModel
    {
        public int TypeMaterialID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Quantidade de dispositivos médicos ligados
        public int ConnectedDevicesCount { get; set; }
    }
}
