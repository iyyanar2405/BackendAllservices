using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleService.Models.Entities
{
    [Table("FindingResponses")]
    public class FindingResponses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("FindingResponseId")]
        public int FindingResponseId { get; set; }

        [Column("FindingId")]
        public int FindingId { get; set; }

        [Column("ResponseText")]
        public required string ResponseText { get; set; }

        [Column("ResponseType")]
        public required string ResponseType { get; set; }

        [Column("ResponseDate")]
        public DateTime ResponseDate { get; set; }

        [Column("RespondedBy")]
        public int RespondedBy { get; set; }

        [Column("IsSubmittedToDNV")]
        public bool IsSubmittedToDNV { get; set; }

        [Column("SubmissionDate")]
        public DateTime? SubmissionDate { get; set; }

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

        [Column("AttachmentPath")]
        public string? AttachmentPath { get; set; }

        [Column("Status")]
        public string? Status { get; set; }

        [Column("ReviewComments")]
        public string? ReviewComments { get; set; }

        [Column("ReviewedBy")]
        public int? ReviewedBy { get; set; }

        [Column("ReviewDate")]
        public DateTime? ReviewDate { get; set; }
    }
}
