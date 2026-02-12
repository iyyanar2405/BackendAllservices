namespace ActionService.Models
{
    public class ActionSiteNode
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public List<ActionSiteNode> Children { get; set; } = new();
    }
}
