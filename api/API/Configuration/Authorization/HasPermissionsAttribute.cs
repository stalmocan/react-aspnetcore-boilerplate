using Library.Security;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Configuration.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class HasPermissionsAttribute : AuthorizeAttribute
    {
        public List<Permissions> Permissions
        {
            get
            {
                return Policy.Substring(CustomAuthConstants.POLICY_PREFIX.Length).Split(",").Select(x => Enum.Parse<Permissions>(x)).ToList();
            }
            set
            {
                Policy = $"{CustomAuthConstants.POLICY_PREFIX}{string.Join(',', value)}";
            }
        }

        public HasPermissionsAttribute(params Permissions[] permissions)
        {
            Permissions = permissions.ToList();
        }
    }
}
