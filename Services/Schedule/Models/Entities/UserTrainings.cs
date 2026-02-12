using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("UserTrainings")]
    public class UserTrainings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("UserTrainingId")]
        public int UserTrainingId { get; set; }

        [Column("UserId")]
        public int UserId { get; set; }

        [Column("TrainingId")]
        public int TrainingId { get; set; }

        [Column("Status")]
        public required string Status { get; set; }

        [Column("EnrollmentDate")]
        public DateTime EnrollmentDate { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Column("DueDate")]
        public DateTime? DueDate { get; set; }

        [Column("Score")]
        public int? Score { get; set; }

        [Column("AttemptCount")]
        public int AttemptCount { get; set; }

        [Column("MaxAttempts")]
        public int? MaxAttempts { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [Column("CreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("ModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("Progress")]
        public int Progress { get; set; }

        [Column("TimeSpent")]
        public int? TimeSpent { get; set; }

        [Column("CertificateIssued")]
        public bool CertificateIssued { get; set; }

        [Column("CertificateNumber")]
        public string? CertificateNumber { get; set; }

        [Column("CertificateIssuedDate")]
        public DateTime? CertificateIssuedDate { get; set; }

        [Column("CertificateExpiryDate")]
        public DateTime? CertificateExpiryDate { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
    }
}
