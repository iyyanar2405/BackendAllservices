using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("UserCityAccess")]
    public class UserCityAccess
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserCityAccessId")]
        public int UserCityAccessId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("CityId")]
        public int CityId { get; set; }

        [Column("AccessLevel")]
        public required string AccessLevel { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("GrantedDate")]
        public DateTime GrantedDate { get; set; }

        [Column("ExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("GrantedBy")]
        public int? GrantedBy { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }
    }
}
