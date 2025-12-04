using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class AvaliacaoFisica
    {
        public int AvaliacaoFisicaId { get; set; }

        [Required(ErrorMessage = "A data da medição é obrigatória")]
        [DataType(DataType.Date)]
        public DateTime DataMedicao { get; set; } = DateTime.Now;

        // --- COMPOSIÇÃO CORPORAL - Obrigatorio ---
        [Required(ErrorMessage = "O Peso é obrigatório")]
        [Range(20, 300, ErrorMessage = "Insira um peso válido (20kg - 300kg)")]
        public float Peso { get; set; }

        [Required(ErrorMessage = "A Altura é obrigatória")]
        [Range(50, 250, ErrorMessage = "Insira uma altura válida em cm (50cm - 250cm)")]
        public float Altura { get; set; }

        [Required(ErrorMessage = "A Gordura Corporal é obrigatória")] 
        [Range(1, 60, ErrorMessage = "Percentagem de gordura inválida")]
        public float GorduraCorporal { get; set; }

        [Required(ErrorMessage = "A Massa Muscular é obrigatória")]
        public float MassaMuscular { get; set; }

        // --- PERÍMETROS (cm) - Opcionais ---

        [Range(10, 200)]
        public float? Pescoco { get; set; }

        [Range(30, 250)]
        public float? Ombros { get; set; }

        [Range(30, 200)]
        public float? Peitoral { get; set; }

        public float? BracoDireito { get; set; }

        public float? BracoEsquerdo { get; set; }

        public float? Cintura { get; set; }

        public float? Abdomen { get; set; }

        public float? Anca { get; set; }

        public float? CoxaDireita { get; set; }

        public float? CoxaEsquerda { get; set; }

        public float? GemeoDireito { get; set; }

        public float? GemeoEsquerdo { get; set; }

        // Chave Estrangeira para UtenteGrupo7
        public int UtenteGrupo7Id { get; set; }
        public UtenteGrupo7? UtenteGrupo7 { get; set; }
    }
}