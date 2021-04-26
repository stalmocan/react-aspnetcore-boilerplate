using Library.Security;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace API.Configuration.Authorization
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public List<Permissions> Permissions { get; private set; }

        public PermissionsRequirement(List<Permissions> permissions)
        {
            Permissions = permissions;
        }
    }
}
