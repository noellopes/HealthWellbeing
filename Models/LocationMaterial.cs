using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LocationMaterial
    {

        public int LocationMaterialID { get; set; }
        
        public string Setor { get; set; }
        [Required(ErrorMessage = "O tipo de setor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O setor não pode exceder 100 caracteres.")]
        public string Sala { get; set; }
        [Required(ErrorMessage = "O mome da sala é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da sala não pode exceder 100 caracteres.")]
        public string Armario { get; set; }
        [Required(ErrorMessage = "O nome do armário é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do armário não pode exceder 100 caracteres.")]
        public string Gaveta { get; set; }
        [Required(ErrorMessage = "O número da gaveta é obrigatório.")]
        [StringLength(50, ErrorMessage = "A gaveta não pode exceder 50 caracteres.")]
        public string Prateleira { get; set; }
        [Required(ErrorMessage = "O número ou nome da prateleira é obrigatório.")]
        [StringLength(50, ErrorMessage = "A prateleira não pode exceder 50 caracteres.")]
        public string CodigoIdentificacao { get; set; }
        [Required(ErrorMessage = "O código de identificação é obrigatório.")]
        [StringLength(50, ErrorMessage = "O código de identificação não pode exceder 50 caracteres.")]
        public string Observacao { get; set; }
        


    }
}
