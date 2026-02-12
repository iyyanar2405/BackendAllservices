using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingsService.Data.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public int UserId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}
