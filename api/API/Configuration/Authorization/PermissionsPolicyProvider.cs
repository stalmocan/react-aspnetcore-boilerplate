using Library.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Configuration.Authorization
{
    public class PermissionsPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; set; }

        public PermissionsPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(CustomAuthConstants.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var permissionsValue = policyName.Substring(CustomAuthConstants.POLICY_PREFIX.Length);

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionsRequirement(permissionsValue.Split(",").Select(x => Enum.Parse<Permissions>(x)).ToList()));

                return Task.FromResult(policy.Build());
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
