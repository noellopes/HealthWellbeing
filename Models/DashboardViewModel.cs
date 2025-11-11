using System.Collections.Generic;

namespace HealthWellbeing.Models
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }  // Changed from TotalClients
        public int TotalProfessionals { get; set; }
        public List<TherapySession> UpcomingSessions { get; set; } = [];
        public List<MoodEntry> RecentMoodEntries { get; set; } = [];
        public List<CrisisAlert> ActiveCrisisAlerts { get; set; } = [];
        public List<TherapySession> TodaysSessions { get; set; } = [];
    }
}