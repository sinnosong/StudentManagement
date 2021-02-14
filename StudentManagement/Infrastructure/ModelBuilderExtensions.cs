using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //指定实体在数据库中生成的名称
            modelBuilder.Entity<Course>().ToTable("Course", "School");
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourse", "School");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.TeacherID });
        }
    }
}