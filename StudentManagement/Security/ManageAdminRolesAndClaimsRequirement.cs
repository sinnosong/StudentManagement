using Microsoft.AspNetCore.Authorization;

namespace StudentManagement.Security
{
    /// <summary>
    /// 管理Admin角色与声明
    /// </summary>
    public class ManageAdminRolesAndClaimsRequirement : IAuthorizationRequirement
    {
    }
}