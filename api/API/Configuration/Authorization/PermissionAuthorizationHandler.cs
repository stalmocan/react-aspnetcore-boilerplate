using Library.Security;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Configuration.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        public PermissionAuthorizationHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
        {
            var permissionsClaim = context.User.FindFirst(CustomAuthConstants.CUSTOM_CLAIM_TYPE);

            if (permissionsClaim != null && !string.IsNullOrWhiteSpace(permissionsClaim.Value))
            {
                var requiredPermissions = permissionsClaim.Value.Split(",").Select(x => Enum.Parse<Permissions>(x)).ToList();

                if (requirement.Permissions.Intersect(requiredPermissions).Any())
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
