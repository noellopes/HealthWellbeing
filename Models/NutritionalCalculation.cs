// Models/NutritionalCalculation.cs
using System;
using System.Linq;

namespace HealthWellbeing.Models
{
    public class NutritionalCalculation
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }

        // Dados do cliente
        public double WeightKg { get; set; }
        public int HeightCm { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public double ActivityFactor { get; set; }
        public string GoalType { get; set; }

        // Resultados do cálculo
        public double BMR { get; set; }
        public double TDEE { get; set; }
        public double DailyCalories { get; set; }
        public double DailyProtein { get; set; }
        public double DailyCarbs { get; set; }
        public double DailyFat { get; set; }

        // Micronutrientes
        public double VitaminC_mg { get; set; }
        public double Calcium_mg { get; set; }
        public double Iron_mg { get; set; }
        public double VitaminD_mcg { get; set; }
        public double Fiber_g { get; set; }

        // Percentuais
        public double ProteinPercentage { get; set; }
        public double CarbsPercentage { get; set; }
        public double FatPercentage { get; set; }

        public double BMI { get; set; }
        public string BMICategory { get; set; }

        public static NutritionalCalculation CalculateForClient(Client client, Goal goal = null)
        {
            if (client == null) return null;

            var calculation = new NutritionalCalculation
            {
                ClientId = client.ClientId,
                ClientName = client.Name,
                WeightKg = client.WeightKg,
                HeightCm = client.HeightCm,
                Gender = client.Gender,
                ActivityFactor = client.ActivityFactor,
                GoalType = goal?.GoalName ?? "Maintenance"
            };

            // Usa a propriedade Age do Client que já foi definida
            calculation.Age = client.Age;

            // Calcular BMR (Mifflin-St Jeor)
            calculation.BMR = CalculateBMR(client.WeightKg, client.HeightCm, calculation.Age, client.Gender);

            // Calcular TDEE
            calculation.TDEE = calculation.BMR * client.ActivityFactor;

            // Ajustar para objetivo
            ApplyGoalAdjustments(calculation);

            // Calcular micronutrientes
            CalculateMicronutrients(calculation);

            // Calcular IMC
            calculation.BMI = CalculateBMI(client.WeightKg, client.HeightCm);
            calculation.BMICategory = GetBMICategory(calculation.BMI);

            return calculation;
        }

        private static double CalculateBMR(double weightKg, int heightCm, int age, string gender)
        {
            if (gender?.ToLower() == "male")
                return (10 * weightKg) + (6.25 * heightCm) - (5 * age) + 5;
            else
                return (10 * weightKg) + (6.25 * heightCm) - (5 * age) - 161;
        }

        private static void ApplyGoalAdjustments(NutritionalCalculation calc)
        {
            string goalLower = calc.GoalType?.ToLower() ?? "";

            if (goalLower.Contains("loss") || goalLower.Contains("perda"))
            {
                calc.DailyCalories = calc.TDEE * 0.85; // 15% de déficit
                calc.ProteinPercentage = 30;
                calc.CarbsPercentage = 40;
                calc.FatPercentage = 30;
            }
            else if (goalLower.Contains("gain") || goalLower.Contains("ganho"))
            {
                calc.DailyCalories = calc.TDEE * 1.15; // 15% de superávit
                calc.ProteinPercentage = 35;
                calc.CarbsPercentage = 45;
                calc.FatPercentage = 20;
            }
            else // manutenção
            {
                calc.DailyCalories = calc.TDEE;
                calc.ProteinPercentage = 25;
                calc.CarbsPercentage = 50;
                calc.FatPercentage = 25;
            }

            calc.DailyProtein = Math.Round((calc.DailyCalories * (calc.ProteinPercentage / 100)) / 4, 1);
            calc.DailyCarbs = Math.Round((calc.DailyCalories * (calc.CarbsPercentage / 100)) / 4, 1);
            calc.DailyFat = Math.Round((calc.DailyCalories * (calc.FatPercentage / 100)) / 9, 1);
        }

        private static void CalculateMicronutrients(NutritionalCalculation calc)
        {
            // Vitamina C
            calc.VitaminC_mg = calc.Gender?.ToLower() == "male" ? 90 : 75;

            // Cálcio
            calc.Calcium_mg = calc.Age < 50 ? 1000 : 1200;

            // Ferro
            if (calc.Gender?.ToLower() == "male")
                calc.Iron_mg = 8;
            else if (calc.Age < 50)
                calc.Iron_mg = 18;
            else
                calc.Iron_mg = 8;

            // Vitamina D
            calc.VitaminD_mcg = 15;

            // Fibra (14g por 1000kcal)
            calc.Fiber_g = Math.Round(calc.DailyCalories / 1000 * 14, 1);
        }

        private static double CalculateBMI(double weightKg, int heightCm)
        {
            if (heightCm == 0) return 0;
            double heightM = heightCm / 100.0;
            return Math.Round(weightKg / (heightM * heightM), 1);
        }

        private static string GetBMICategory(double bmi)
        {
            if (bmi < 18.5) return "Abaixo do peso";
            if (bmi < 25) return "Peso normal";
            if (bmi < 30) return "Sobrepeso";
            return "Obesidade";
        }
    }
}