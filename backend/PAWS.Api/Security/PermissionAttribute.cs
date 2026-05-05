using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PAWS.Api.Security
{
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public RequirePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentUser = context.HttpContext.RequestServices.GetService<ICurrentUserService>();

            if (currentUser == null || !currentUser.User.Permissions.Contains(_permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
