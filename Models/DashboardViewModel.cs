using System.Collections.Generic;

namespace HealthWellbeing.Models
{
    public class DashboardViewModel
    {
        public int TotalClients { get; set; }
        public int TotalProfessionals { get; set; }
        public List<TherapySession> UpcomingSessions { get; set; } = new();
        public List<MoodEntry> RecentMoodEntries { get; set; } = new();
        public List<CrisisAlert> ActiveCrisisAlerts { get; set; } = new();
        public List<TherapySession> TodaysSessions { get; set; } = new();
    }
}