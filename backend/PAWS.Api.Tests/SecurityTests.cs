using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PAWS.Api.Security;
using Xunit;

namespace PAWS.Api.Tests
{
    public class SecurityTests
    {
        [Fact]
        public void RequirePermission_ForbidsUserWithoutPermission()
        {
            var services = new ServiceCollection();
            services.AddScoped<ICurrentUserService>(_ => new CurrentUserService
            {
                User = new CurrentUser
                {
                    Email = "advisor@missouri.edu",
                    Permissions = new List<string> { "Students.View" }
                }
            });
            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext { RequestServices = provider };
            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext
            {
                HttpContext = httpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var attribute = new RequirePermissionAttribute("Reports.Export");
            attribute.OnAuthorization(context);

            Assert.IsType<ForbidResult>(context.Result);
        }

        [Fact]
        public void RequirePermission_AllowsUserWithPermission()
        {
            var services = new ServiceCollection();
            services.AddScoped<ICurrentUserService>(_ => new CurrentUserService
            {
                User = new CurrentUser
                {
                    Email = "reporter@missouri.edu",
                    Permissions = new List<string> { "Reports.Export" }
                }
            });
            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext { RequestServices = provider };
            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext
            {
                HttpContext = httpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };
            var context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            var attribute = new RequirePermissionAttribute("Reports.Export");
            attribute.OnAuthorization(context);

            Assert.Null(context.Result);
        }
    }
}
