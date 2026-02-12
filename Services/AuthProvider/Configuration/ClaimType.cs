namespace AuthProvider.Configuration
{
    enum ClaimType
    {
        Chorus_Role,
        Scope,
        Module,
        Service,
        Page,
        Resource
    }

    public class AuthProviderClaims
    {
        public bool ValidateClaimType(string claim)
        {
            var t = Enum.IsDefined(typeof(ClaimType), claim);
            return Enum.IsDefined(typeof(ClaimType), claim);
        }

        public string[] ListAllClaims()
        {
            return Enum.GetNames(typeof(ClaimType));
        }

    }

}
