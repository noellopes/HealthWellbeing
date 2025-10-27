﻿using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Member
    {   
        [Key]
        public int MemberId { get; set; } = default!;

        public int ClientId { get; set; } = default;
        public Client? Client { get; set; } = default;
    }
}
