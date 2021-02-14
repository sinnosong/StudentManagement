using StudentManagement.Dtos;
using StudentManagement.Models;
using System.Collections.Generic;

namespace StudentManagement.ViewModels
{
    public class TeacherListViewModel
    {
        public PagedResultDto<Teacher> Teachers { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }

        /// <summary>
        /// 选中教师ID
        /// </summary>
        public int SelectedId { get; set; }

        /// <summary>
        /// 选中课程ID
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}