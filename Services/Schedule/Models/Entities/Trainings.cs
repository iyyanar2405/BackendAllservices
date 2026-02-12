using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("Trainings")]
    public class Trainings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TrainingId")]
        public int TrainingId { get; set; }

        [Column("TrainingName")]
        public required string TrainingName { get; set; }

        [Column("TrainingCode")]
        public required string TrainingCode { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("TrainingType")]
        public string? TrainingType { get; set; }

        [Column("Category")]
        public string? Category { get; set; }

        [Column("Duration")]
        public int? Duration { get; set; }

        [Column("DueDate")]
        public DateTime? DueDate { get; set; }

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

        [Column("Prerequisites")]
        public string? Prerequisites { get; set; }

        [Column("LearningObjectives")]
        public string? LearningObjectives { get; set; }

        [Column("Materials")]
        public string? Materials { get; set; }

        [Column("AssessmentRequired")]
        public bool AssessmentRequired { get; set; }

        [Column("PassingScore")]
        public int? PassingScore { get; set; }

        [Column("CertificateIssued")]
        public bool CertificateIssued { get; set; }

        [Column("ValidityPeriod")]
        public int? ValidityPeriod { get; set; }

        [Column("Cost")]
        public decimal? Cost { get; set; }

        [Column("Currency")]
        public string? Currency { get; set; }
    }
}
