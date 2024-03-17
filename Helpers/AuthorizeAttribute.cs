using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ChatingApp.Models;
using Microsoft.IdentityModel.Tokens;

namespace ChatingApp.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private string? _roles;
        
        public AuthorizeAttribute()
        {
            // Accept all roles to access!!
        }

        public AuthorizeAttribute(string? roles)
        {
            if (!roles.IsNullOrEmpty()) _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.Items["User"] as Account;
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (_roles != null && user?.Roles != _roles) {
                // have no right to do
                context.Result = new JsonResult(new { message = "You can not access this" }) { StatusCode = StatusCodes.Status406NotAcceptable };
            }
        }
    }
}
