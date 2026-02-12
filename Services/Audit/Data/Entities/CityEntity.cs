namespace AuditService.Data.Entities
{
    public class CityEntity
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int CountryId { get; set; }
    }
}
