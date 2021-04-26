using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;

namespace Library.Security
{
    public class SecurityContext
    {
        public IHttpContextAccessor HttpContextAccessor { get; }

        public SecurityContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public SecurityDetails CallingUserDetails
        {
            get 
            {
                var roleClaimValue = HttpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrWhiteSpace(roleClaimValue))
                    return null;
                return JsonConvert.DeserializeObject<SecurityDetails>(roleClaimValue);                
            }
        }
    }
}
