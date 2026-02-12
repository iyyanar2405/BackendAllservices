namespace ContractService.Models
{
    public class OverviewCardResponse
    {
        public List<OverviewServiceData> Data { get; set; } = new();
        public int TotalItems { get; set; }
    }

    public class OverviewServiceData
    {
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public List<OverviewYearData> YearData { get; set; } = new();
    }

    public class OverviewYearData
    {
        public int Year { get; set; }
        public List<OverviewValueData> Values { get; set; } = new();
    }

    public class OverviewValueData
    {
        public int Count { get; set; }
        public int Seq { get; set; }
        public string? StatusValue { get; set; }
        public int TotalCount { get; set; }
    }

    public class OverviewFilter
    {
        public List<int> Companies { get; set; } = new();
        public List<int> Sites { get; set; } = new();
        public List<int> Services { get; set; } = new();
    }
}
