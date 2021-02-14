using StudentManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    public class Student : Person
    {
        /// <summary>
        /// 主修科目
        /// </summary>
        public MajorEnum? Major { get; set; }

        public string Photopath { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}