using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace API.Configuration.Authorization
{
    public class CustomAuthHandler : AuthenticationHandler<CustomOptionsAuth>
    {
        SecurityService SecurityService { get; }

        public CustomAuthHandler(IOptionsMonitor<CustomOptionsAuth> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            SecurityService securityService) : base(options, logger, encoder, clock)
        {
            SecurityService = securityService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authority Header"));
            }

            var encryptedSecurityDetails = Request.Headers["Authorization"].FirstOrDefault();
            if (encryptedSecurityDetails.StartsWith("Bearer"))
                encryptedSecurityDetails = encryptedSecurityDetails.Remove(0, 6).Trim();

            var securityDetails = SecurityService.DecryptSecurityDetails(encryptedSecurityDetails);

            if(securityDetails == null || securityDetails.ExpiresOn < DateTime.Now)
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization token invalid"));
            }

            var claims = new List<Claim>
            {
                new Claim(CustomAuthConstants.CUSTOM_CLAIM_TYPE, string.Join(",", securityDetails.Permissions)),
                new Claim(ClaimTypes.Role, JsonConvert.SerializeObject(securityDetails))
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        } 
    }
}
