namespace ContractService.Models
{
    public class WidgetFinancialStatusResponse
    {
        public string? FinancialStatus { get; set; }
        public int FinancialCount { get; set; }
        public double Financialpercentage { get; set; }
    }

    public class WidgetTrainingDataResponse
    {
        public List<TrainingStatusItem> TrainingData { get; set; } = new();
    }

    public class TrainingStatusItem
    {
        public string? TrainingName { get; set; }
        public string? TrainingStatus { get; set; }
        public string? TrainingDueDate { get; set; }
        public string? TrainingLocation { get; set; }
    }

    public class UpcomingAuditResponse
    {
        public List<string> Confirmed { get; set; } = new();
        public List<string> ToBeConfirmed { get; set; } = new();
        public List<string> ToBeConfirmedByDNV { get; set; } = new();
    }
}
