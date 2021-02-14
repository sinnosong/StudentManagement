using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密码：")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "新密码与确认密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}