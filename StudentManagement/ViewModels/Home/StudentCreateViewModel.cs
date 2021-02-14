using Microsoft.AspNetCore.Http;
using StudentManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class StudentCreateViewModel
    {
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名，不能为空！"), MaxLength(6, ErrorMessage = "最多只能输入6个字")]
        public string Name { get; set; }

        /// <summary>
        /// 主修科目
        /// </summary>
        [Display(Name = "主修科目")]
        [Required]
        public MajorEnum? Major { get; set; }

        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "邮箱格式不正确！")]
        [Required(ErrorMessage = "邮箱不能为空！")]
        public string Email { get; set; }

        [Display(Name = "头像")]
        public List<IFormFile> Photos { get; set; }

        [Display(Name = "入学时间")]
        public DateTime EnrollmentDate { get; set; }
    }
}