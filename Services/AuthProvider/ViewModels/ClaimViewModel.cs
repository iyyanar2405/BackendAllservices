using System.ComponentModel.DataAnnotations;

namespace AuthProvider.ViewModel
{
    public class ClaimViewModel
    {
        public string? UserName { get; set; }
        public string? RoleName { get; set; }

        [Required]
        public string ClaimType { get; set; } = "";

        [Required]
        public string ClaimValue { get; set; } = "";
    }
}
