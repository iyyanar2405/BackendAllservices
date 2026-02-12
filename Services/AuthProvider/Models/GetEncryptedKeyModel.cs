using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthProvider.Models
{
    [Table("GetUser")]
    public class GetEncryptedKeyModel
    {
        [Key]
        public int Site_ID { get; set; }

        public string Username { get; set; }

        public bool? Active { get; set; }

        public string Password { get; set; }

    }
}
