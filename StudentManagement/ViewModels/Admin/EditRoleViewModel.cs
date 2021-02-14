using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        [Display(Name = "角色ID")]
        public string Id { get; set; }

        [Required(ErrorMessage = "角色名称必填")]
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; } //角色名下的用户
    }
}