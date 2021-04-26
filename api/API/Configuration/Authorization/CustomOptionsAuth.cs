using Microsoft.AspNetCore.Authentication;

namespace API.Configuration.Authorization
{
    public class CustomOptionsAuth : AuthenticationSchemeOptions
    {
        public string DisplayMessage { get; set; }
    }
}
