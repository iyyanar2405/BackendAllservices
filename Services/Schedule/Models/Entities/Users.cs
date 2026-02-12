using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("Username")]
        public required string Username { get; set; }

        [Column("Email")]
        public required string Email { get; set; }

        [Column("FirstName")]
        public required string FirstName { get; set; }

        [Column("LastName")]
        public required string LastName { get; set; }

        [Column("PasswordHash")]
        public required string PasswordHash { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("Phone")]
        public string? Phone { get; set; }

        [Column("Position")]
        public string? Position { get; set; }

        [Column("Department")]
        public string? Department { get; set; }

        [Column("TimeZone")]
        public string? TimeZone { get; set; }

        [Column("Language")]
        public string? Language { get; set; }

        [Column("IsEmailVerified")]
        public bool IsEmailVerified { get; set; }

        [Column("EmailVerificationToken")]
        public string? EmailVerificationToken { get; set; }

        [Column("PasswordResetToken")]
        public string? PasswordResetToken { get; set; }

        [Column("PasswordResetExpiry")]
        public DateTime? PasswordResetExpiry { get; set; }

        [Column("TwoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [Column("TwoFactorSecret")]
        public string? TwoFactorSecret { get; set; }
    }
}
