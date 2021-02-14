using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Models;
using StudentManagement.Models.EnumTypes;
using System;
using System.Linq;

namespace StudentManagement.Infrastructure.Data
{
    /// <summary>
    /// 数据初始化
    /// </summary>
    public static class DataInitializer
    {
        public static IApplicationBuilder UseDataInitializer(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                #region 学生种子信息

                if (dbcontext.Students.Any())
                {
                    return builder;
                }
                var students = new[]
                {
                    new Student{ Name="张三",Major=MajorEnum.ComputerScience,Email = "zhangsan@outlook.com",EnrollmentDate=DateTime.Parse("2019-10-3")},
                    new Student{ Name="李四",Major=MajorEnum.Mathematics,Email = "lisi@outlook.com",EnrollmentDate=DateTime.Parse("2020-10-3") },
                    new Student{ Name="王五",Major=MajorEnum.ElectronicCommerce,Email = "wangwu@outlook.com",EnrollmentDate=DateTime.Parse("2020-2-3")}
                };
                foreach (Student item in students)
                {
                    dbcontext.Students.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 学生种子信息

                #region 教师种子信息

                var teachers = new[]
                {
                    new Teacher{Name="张老师",HireDate=DateTime.Parse("2015-12-2") },
                    new Teacher{Name="王老师",HireDate=DateTime.Parse("2016-12-2") },
                    new Teacher{Name="宋老师",HireDate=DateTime.Parse("2017-12-2") },
                    new Teacher{Name="李老师",HireDate=DateTime.Parse("2018-12-2") },
                    new Teacher{Name="苟老师",HireDate=DateTime.Parse("2019-12-2") }
                };
                foreach (var item in teachers)
                {
                    dbcontext.Teachers.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 教师种子信息

                #region 学院种子信息

                var departments = new[]
                {
                    new Department{ Name="a",Budget=350000,StartDate=DateTime.Parse("1999-9-1"),TeacherID=teachers.Single(i=>i.Name=="张老师").Id},
                    new Department{ Name="b",Budget=350000,StartDate=DateTime.Parse("2000-9-1"),TeacherID=teachers.Single(i=>i.Name=="王老师").Id},
                    new Department{ Name="c",Budget=350000,StartDate=DateTime.Parse("2001-9-1"),TeacherID=teachers.Single(i=>i.Name=="宋老师").Id},
                    new Department{ Name="d",Budget=350000,StartDate=DateTime.Parse("1998-9-1"),TeacherID=teachers.Single(i=>i.Name=="苟老师").Id},
                };
                foreach (var item in departments)
                {
                    dbcontext.Departments.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 学院种子信息

                #region 课程种子数据

                if (dbcontext.Courses.Any())
                {
                    return builder;
                }
                var courses = new[]
                {
                    new Course{CourseID=1050,Title="数学",Credits = 3,DepartmentID=departments.Single(s=>s.Name=="b").DepartmentID},
                    new Course{CourseID=4022,Title="政治",Credits = 3 ,DepartmentID=departments.Single(s=>s.Name=="c").DepartmentID},
                    new Course{CourseID=1045,Title="物理",Credits = 4,DepartmentID=departments.Single(s=>s.Name=="a").DepartmentID },
                    new Course{CourseID=5000,Title="生物",Credits = 4 ,DepartmentID=departments.Single(s=>s.Name=="b").DepartmentID},
                    new Course{CourseID=1056,Title="英语",Credits = 5,DepartmentID=departments.Single(s=>s.Name=="d").DepartmentID },
                    new Course{CourseID=1023,Title="历史",Credits = 3,DepartmentID=departments.Single(s=>s.Name=="d").DepartmentID }
                };
                foreach (var course in courses)
                {
                    dbcontext.Courses.Add(course);
                }
                dbcontext.SaveChanges();

                #endregion 课程种子数据

                #region 办公室种子数据

                var OfficeLocations = new[]
                {
                    new OfficeLocation{ TeacherId=teachers.Single(t=>t.Name=="宋老师").Id,Location="X楼"},
                    new OfficeLocation{ TeacherId=teachers.Single(t=>t.Name=="苟老师").Id,Location="Y楼"},
                    new OfficeLocation{ TeacherId=teachers.Single(t=>t.Name=="王老师").Id,Location="Z楼"},
                };
                foreach (var item in OfficeLocations)
                {
                    dbcontext.OfficeLocations.Add(item);
                }
                dbcontext.SaveChanges();

                #endregion 办公室种子数据

                //#region 教师分配课程

                //var courseTeachers = new[]
                //{
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="物理").CourseID,TeacherID=teachers.Single(i=>i.Name=="宋老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="历史").CourseID,TeacherID=teachers.Single(i=>i.Name=="苟老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="物理").CourseID,TeacherID=teachers.Single(i=>i.Name=="王老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="生物").CourseID,TeacherID=teachers.Single(i=>i.Name=="张老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="数学").CourseID,TeacherID=teachers.Single(i=>i.Name=="李老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="政治").CourseID,TeacherID=teachers.Single(i=>i.Name=="宋老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="英语").CourseID,TeacherID=teachers.Single(i=>i.Name=="李老师").Id},
                //    new CourseAssignment{ CourseID = courses.Single(c=>c.Title=="历史").CourseID,TeacherID=teachers.Single(i=>i.Name=="苟老师").Id}
                //};
                //foreach (var item in courseTeachers)
                //{
                //    dbcontext.CourseAssignments.Add(item);
                //}
                //dbcontext.SaveChanges();

                //#endregion 教师分配课程

                #region 学生课程关联种子数据

                var studentCourses = new[]
                {
                    new StudentCourse{ CourseID=courses.Single(i=>i.Title=="历史").CourseID,StudentID = students.First().Id,Grade=Grade.A},
                    new StudentCourse{ CourseID=courses.Single(i=>i.Title=="物理").CourseID,StudentID = students.First().Id,Grade=Grade.B},
                    new StudentCourse{ CourseID=courses.Single(i=>i.Title=="数学").CourseID,StudentID = students.First().Id,Grade=Grade.C},
                    new StudentCourse{ CourseID=courses.Single(i=>i.Title=="政治").CourseID,StudentID = students.First().Id,Grade=Grade.B},
                    new StudentCourse{ CourseID=courses.Single(i=>i.Title=="英语").CourseID,StudentID = students.First().Id,Grade=Grade.A}
                };
                foreach (var sc in studentCourses)
                {
                    dbcontext.StudentCourses.Add(sc);
                }
                dbcontext.SaveChanges();

                #endregion 学生课程关联种子数据

                #region 用户种子数据

                if (dbcontext.Users.Any())
                {
                    return builder;
                }
                var user = new ApplicationUser
                {
                    Email = "sinno.song@outlook.com",
                    UserName = "sinno.song@outlook.com",
                    EmailConfirmed = true,
                    City = "驻马店",
                    Address = "新蔡县"
                };
                userManager.CreateAsync(user, "123456").Wait();
                dbcontext.SaveChanges();

                var adminRole = "Admin";
                var role = new IdentityRole { Name = adminRole };
                dbcontext.Roles.Add(role);
                dbcontext.SaveChanges();

                dbcontext.UserRoles.Add(new IdentityUserRole<string>
                {
                    RoleId = role.Id,
                    UserId = user.Id
                });
                dbcontext.SaveChanges();

                #endregion 用户种子数据
            }
            return builder;
        }
    }
}