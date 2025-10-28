﻿using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TerapeutaModel> Terapeutas { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<AgendamentoModel> Agendamentos { get; set; }
        public DbSet<TipoServico> TipoServicos{ get; set; }
    }
}
