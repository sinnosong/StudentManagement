using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        //[ValidEmailDomain(allowedDomain: "outlook.com", ErrorMessage = "注册邮箱的域名必须是outlook.com")] // 使用自定义的邮箱域名验证
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }

        [StringLength(maximumLength: 30, MinimumLength = 10)]
        public string City { get; set; }

        [StringLength(maximumLength: 30, MinimumLength = 10)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "确认密码")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致，请重新输出")]
        public string ConfirmPassword { get; set; }
    }
}