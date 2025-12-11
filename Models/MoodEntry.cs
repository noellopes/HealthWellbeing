using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class MoodEntry
    {
        [Key]
        public int MoodEntryId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Entry date and time is required.")]
        [Display(Name = "Entry Date/Time")]
        public DateTime EntryDateTime { get; set; }

        [Required(ErrorMessage = "Mood score is required.")]
        [Range(1, 10, ErrorMessage = "Mood score must be between 1 and 10.")]
        [Display(Name = "Mood Score (1-10)")]
        public int MoodScore { get; set; }

        [Required(ErrorMessage = "Please select an emotion.")]
        [Display(Name = "Primary Emotion")]
        public EmotionType Emotion { get; set; }

        [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Trigger (Optional)")]
        public TriggerType? Trigger { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }

    public enum EmotionType
    {
        [Display(Name = "😊 Happy")]
        Happy,

        [Display(Name = "😢 Sad")]
        Sad,

        [Display(Name = "😰 Anxious")]
        Anxious,

        [Display(Name = "😠 Angry")]
        Angry,

        [Display(Name = "😌 Calm")]
        Calm,

        [Display(Name = "😫 Stressed")]
        Stressed,

        [Display(Name = "😊 Content")]
        Content,

        [Display(Name = "😵 Overwhelmed")]
        Overwhelmed,

        [Display(Name = "🙂 Hopeful")]
        Hopeful,

        [Display(Name = "😨 Fearful")]
        Fearful,

        [Display(Name = "😔 Depressed")]
        Depressed,

        [Display(Name = "😤 Frustrated")]
        Frustrated,

        [Display(Name = "😐 Neutral")]
        Neutral,

        [Display(Name = "😴 Tired")]
        Tired,

        [Display(Name = "⚡ Energized")]
        Energized
    }

    public enum TriggerType
    {
        [Display(Name = "No Specific Trigger")]
        None,

        [Display(Name = "Work/School Stress")]
        WorkStress,

        [Display(Name = "Relationship Issues")]
        RelationshipIssues,

        [Display(Name = "Financial Concerns")]
        FinancialConcerns,

        [Display(Name = "Health Issues")]
        HealthIssues,

        [Display(Name = "Family Problems")]
        FamilyProblems,

        [Display(Name = "Social Situations")]
        SocialSituations,

        [Display(Name = "Sleep Problems")]
        SleepProblems,

        [Display(Name = "Poor Diet/Nutrition")]
        PoorDiet,

        [Display(Name = "Lack of Exercise")]
        LackOfExercise,

        [Display(Name = "Medication Changes")]
        MedicationChanges,

        [Display(Name = "Substance Use")]
        SubstanceUse,

        [Display(Name = "Trauma/Past Events")]
        Trauma,

        [Display(Name = "Weather/Seasonal Changes")]
        Weather,

        [Display(Name = "Loneliness/Isolation")]
        Loneliness,

        [Display(Name = "Other/Unknown")]
        Other
    }
}