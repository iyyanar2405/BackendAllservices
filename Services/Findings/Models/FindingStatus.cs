using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindingsAPI.Gateway
{
    [Table("FindingStatuses")]
    public class FindingStatus
    {
        [Key]
        public int FindingStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsClosedStatus { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
