namespace NeoEvaluation.API.DTOs
{
    public class DashboardStatsDto
    {
        public int TalentsActifs { get; set; }
        public double TauxReussite { get; set; }
        public int TestsEnCours { get; set; }
        public double ScoreIaMoyen { get; set; }
        
        public List<ActivityDto> RecentActivities { get; set; } = new();
        public List<ChartDataDto> ChartData { get; set; } = new();
        public string AiInsight { get; set; } = string.Empty;
    }

    public class ActivityDto
    {
        public string User { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Color { get; set; } = "#fbbf24";
    }

    public class ChartDataDto
    {
        public string Day { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}