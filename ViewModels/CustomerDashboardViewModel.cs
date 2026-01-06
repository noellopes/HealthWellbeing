using HealthWellbeing.Models;
using System.Collections.Generic;

namespace HealthWellbeing.ViewModels
{
    public class CustomerDashboardViewModel
    {
        // Dados do Cliente
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int TotalPoints { get; set; }

        // Dados do Nível
        public Level CurrentLevel { get; set; }
        public Level NextLevel { get; set; }
        public int ProgressPercentage { get; set; }
        public int PointsNeeded { get; set; }
        public bool IsMaxLevel { get; set; }

        // Listas de Dados
        public List<CustomerActivity> RecentActivities { get; set; }
        public List<CustomerEvent> ActiveEnrollments { get; set; }

        // Dados dos Badges
        public int BadgesEarnedCount { get; set; }
        public int BadgesTotalCount { get; set; }
        public List<Badge> RecentBadges { get; set; }
    }
}