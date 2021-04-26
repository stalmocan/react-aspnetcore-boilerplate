namespace Library.Security
{
    public class AuthorizationToken
    {
        public string EncryptedAuthorizationToken { get; set; }
        public SecurityDetails SecurityDetails { get; set; }
    }
}
