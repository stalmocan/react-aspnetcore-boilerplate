using API.Configuration.Authorization;
using Library;
using Library.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ExamplesController : ControllerBase
    {
        private SecurityService SecurityService { get; }
        private AppConfigData AppConfigData { get; }
        private SecurityContext SecurityContext { get; }

        public ExamplesController(SecurityService securityService, SecurityContext securityContext, IOptions<AppConfigData> appConfigData)
        {
            SecurityService = securityService;
            AppConfigData = appConfigData.Value;
            SecurityContext = securityContext;
        }

        [Route("AuthorizeAsUser")]
        [AllowAnonymous] //because [Authorize] is specified on controller
        [HttpGet]
        public AuthorizationToken AuthorizeAsUser()
        {
            return SecurityService.CreateAuthorizationToken(new CreateAuthorizationTokenDto
            {
                UserId = 1,
                Permissions = new List<Permissions> { Permissions.User }
            });
        }

        [Route("AuthorizeAsAdmin")]
        [AllowAnonymous] //because [Authorize] is specified on controller
        [HttpGet]
        public AuthorizationToken AuthorizeAsAdmin()
        {
            return SecurityService.CreateAuthorizationToken(new CreateAuthorizationTokenDto
            {
                UserId = 2,
                Permissions = new List<Permissions> { Permissions.Admin }
            });
        }

        [Route("DoSomeProcessing")]
        [AllowAnonymous] //because [Authorize] is specified on controller
        [HttpGet]
        public bool DoSomeProcessing()
        {
            return true;
        }

        // endpoint is secured automatically because [Authorize] is specified on controller
        [Route("DoSomeProcessing/Secured")]
        [HttpGet]
        public int DoSomeProcessingSecuredForAnyone()
        {
            var callingUserId = SecurityContext.CallingUserDetails.UserId;

            return callingUserId;
        }

        [HasPermissions(Permissions.User)] //restricts endpoint only for users with the "User" permission
        [Route("DoSomeProcessing/Secured/User")]
        [HttpGet]
        public string DoSomeProcessingSecuredForUser()
        {
            var connectionStringFromAppsettingsJson = AppConfigData.ConnectionString;

            return connectionStringFromAppsettingsJson;
        }

        [HasPermissions(Permissions.Admin)] //restricts endpoint only for users with the "Admin" permission
        [Route("DoSomeProcessing/Secured/Admin")]
        [HttpGet]
        public string DoSomeProcessingSecuredForAdmin()
        {
            var connectionStringFromAppsettingsJson = AppConfigData.ConnectionString;

            return connectionStringFromAppsettingsJson;
        }
    }
}
