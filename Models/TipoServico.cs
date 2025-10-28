﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoServico
    {
        [Key]
        public int TipoServicoId { get; set; }

        [Required(ErrorMessage = "O nome do tipo de serviço é obrigatório.")]
        public string Nome { get; set; }         // Ex.: "massagens", "banhos", "tratamentos", "fisioterapia", "Programas de bem-estar"
        public string? Descricao { get; set; }    // Detalhes do tipo de serviço

        // Relacionamento 1:N com os serviços específicos
        //public ICollection<Servico> Servicos { get; set; }
    }
}
