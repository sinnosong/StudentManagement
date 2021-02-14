using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; } // 用户在身份认证前访问的URL
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}