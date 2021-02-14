namespace StudentManagement.ViewModels
{
    /// <summary>
    /// 用户拥有的角色列表
    /// </summary>
    public class RolesInUserViewModel
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}