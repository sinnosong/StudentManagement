using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentManagement.Security
{
    /// <summary>
    /// 只有编辑其他Admin角色和声明的处理程序
    /// </summary>
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            // 获取Http上下文
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = _httpContextAccessor.HttpContext.Request.Query["userId"];
            // 判断用户是否用于Admin角色，并且拥有claim.Type=="Edit Role"且值为true
            if (context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true"))
            {
                // 如果当前拥有Admin角色的UserId为空，则说明进入的是角色列表页面
                // 无需判断当前登录用户的Id
                if (string.IsNullOrEmpty(adminIdBeingEdited))
                {
                    context.Succeed(requirement);
                }
                else if (adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}