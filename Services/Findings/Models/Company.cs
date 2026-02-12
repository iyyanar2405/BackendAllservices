// Models/Company.cs
namespace FindingsAPI.Gateway
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string Status { get; set; }
    }
}