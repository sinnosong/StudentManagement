using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Dtos
{
    public class EnrollmentDateGroupDto
    {
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EnrollmentDate { get; set; }

        public int StudentCount { get; set; }
    }
}