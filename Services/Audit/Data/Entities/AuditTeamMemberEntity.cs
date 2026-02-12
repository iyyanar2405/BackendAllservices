namespace AuditService.Data.Entities
{
    public class AuditTeamMemberEntity
    {
        public int AuditTeamMemberId { get; set; }
        public int AuditId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
