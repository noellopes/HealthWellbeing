﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class HealthWellbeingDbContext : DbContext
    {
        public HealthWellbeingDbContext(DbContextOptions<HealthWellbeingDbContext> options) : base(options) { }

        public DbSet<HealthWellbeing.Models.Alergia> Alergia { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.RestricaoAlimentar> RestricaoAlimentar { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Receita> Receita { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Food> Food { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodCategory> FoodCategory { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.Objective> Objective { get; set; } = default!;
        public DbSet<HealthWellbeing.Models.FoodPortion> FoodPortion { get; set; } = default!;
    }
}
