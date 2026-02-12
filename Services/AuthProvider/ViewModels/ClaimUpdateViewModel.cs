using System.ComponentModel.DataAnnotations;

namespace AuthProvider.ViewModel
{
    public class ClaimUpdateViewModel : ClaimViewModel
    {

        [Required]
        public string OriginalClaimType { get; set; } = "";

        [Required]
        public string OriginalClaimValue { get; set; } = "";
    }
}
