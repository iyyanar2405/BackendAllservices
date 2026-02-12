namespace ActionService.Models
{
    public class ActionsPaginationResponse
    {
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public List<ActionItem> Items { get; set; } = new();
    }
}
