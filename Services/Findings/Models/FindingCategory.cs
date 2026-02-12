using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FindingsAPI.Gateway
{
    [Table("FindingCategories")]
    public class FindingCategory
    {
        [Key]
        public int FindingCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ParentCategoryId { get; set; }
        public string? Color { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
