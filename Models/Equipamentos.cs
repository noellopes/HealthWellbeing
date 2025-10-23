using System.ComponentModel.DataAnnotations;

namespace ProjetoSaude.Models
{
    public class Equipamento
    {
        public int EquipamentoId { get; set; }

        [Required(ErrorMessage = "O nome do equipamento é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O tipo de equipamento é obrigatório")]
        [StringLength(100, ErrorMessage = "O tipo não pode ter mais de 100 caracteres")]
        [Display(Name = "Tipo de Equipamento")]
        public string Tipo { get; set; }

        [StringLength(200, ErrorMessage = "A descrição não pode ter mais de 200 caracteres")]
        public string Descricao { get; set; }

        [Display(Name = "Disponível?")]
        public bool Disponivel { get; set; } = true; // por defeito está disponível

        

        
    }
}
