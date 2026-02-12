namespace AuthProvider.Models
{
    public class UserTfaInfo
    {            
        public bool isTwoFactor { get; set; }        
        public bool isTfaLogin { get; set; }        
        public int methodId { get; set; }        
        public string mobNum { get; set; }
    }
}
