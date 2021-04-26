using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace API.Configuration
{
    public class SwaggerSecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var requiredScopes = context.MethodInfo
                     .GetCustomAttributes(true)
                     .OfType<AuthorizeAttribute>()
                     .Select(attr => attr.Policy)
                     .Distinct();

            if (requiredScopes.Any())
            {
                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                 {
                     new OpenApiSecurityRequirement
                     {
                         [ oAuthScheme ] = requiredScopes.ToList()
                     }
                 };

            }
        }
    }
}