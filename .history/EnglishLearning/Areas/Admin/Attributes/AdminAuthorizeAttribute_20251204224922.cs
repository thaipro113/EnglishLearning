using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EnglishLearning.Areas.Admin.Attributes
{
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Kiểm tra xem user đã đăng nhập với AdminScheme chưa
            if (user.Identity?.IsAuthenticated != true || 
                !user.HasClaim(c => c.Type == "AuthScheme" && c.Value == "AdminScheme"))
            {
                // Chưa đăng nhập hoặc không phải admin scheme -> redirect về trang login admin
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "Admin", returnUrl = context.HttpContext.Request.Path });
            }
        }
    }
}
