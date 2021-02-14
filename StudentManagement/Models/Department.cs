using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Models
{
    /// <summary>
    /// 学院
    /// </summary>
    public class Department
    {
        public int DepartmentID { get; set; }

        [Display(Name = "学院名称")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "预算")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "成立时间")]
        public DateTime StartDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Display(Name = "负责人")]
        public int? TeacherID { get; set; }

        /// <summary>
        /// 学院主任
        /// </summary>
        public Teacher Administrator { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}