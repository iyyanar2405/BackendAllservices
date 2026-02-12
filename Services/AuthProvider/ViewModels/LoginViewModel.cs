using System.ComponentModel.DataAnnotations;

namespace AuthProvider.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string DeviceCode { get; set; }


    }
}
