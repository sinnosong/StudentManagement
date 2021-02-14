using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Services.Interface;
using StudentManagement.Models;
using StudentManagement.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<CourseAssignment, int> _courseAssignmentRepository;
        private readonly AppDbContext _dbContext;

        public CourseController(ICourseService courseService, IRepository<Department, int> departmentRepository,
            IRepository<Course, int> courseRepository, IRepository<CourseAssignment, int> courseAssignmentRepository, AppDbContext dbContext)
        {
            this._courseService = courseService;
            this._departmentRepository = departmentRepository;
            this._courseRepository = courseRepository;
            this._courseAssignmentRepository = courseAssignmentRepository;
            this._dbContext = dbContext;
        }

        #region 添加课程

        public IActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseID = input.CourseID,
                    Title = input.Title,
                    Credits = input.Credits,
                    DepartmentID = input.DepartmentID
                };
                await _courseRepository.InsertAsync(course);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _departmentRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }

        #endregion 添加课程

        public async Task<IActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPageinatedResult(input);
            return View(models);
        }

        public IActionResult Edit(int? courseId)
        {
            if (!courseId.HasValue)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试";
                return View("NotFound");
            }
            var course = _courseRepository.FirstOrDefault(a => a.CourseID == courseId);
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试";
                return View("NotFound");
            }
            var dtos = DepartmentsDropDownList(course.DepartmentID); // 将学院列表中选中的值修改为true
            CourseCreateViewModel courseCreateViewModel = new CourseCreateViewModel
            {
                DepartmentList = dtos,
                CourseID = course.CourseID,
                Credits = course.Credits,
                Title = course.Title,
                DepartmentID = course.DepartmentID
            };
            return View(courseCreateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CourseCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var course = _courseRepository.FirstOrDefault(a => a.CourseID == input.CourseID);
                if (course != null)
                {
                    course.CourseID = input.CourseID;
                    course.Title = input.Title;
                    course.DepartmentID = input.DepartmentID;
                    course.Credits = input.Credits;
                    await _courseRepository.UpdateAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = $"课程编号{input.CourseID}的信息不存在，请重试";
                    return View("NotFound");
                }
            }
            return View(input);
        }

        public async Task<ViewResult> Details(int courseId)
        {
            var course = await _courseRepository.GetAll().Include(a => a.Department).FirstOrDefaultAsync(a => a.CourseID == courseId);
            if (course == null)
            {
                ViewBag.ErrorMessage = $"课程编号{courseId}的信息不存在，请重试";
                return View("NotFound");
            }
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _courseRepository.FirstOrDefaultAsync(a => a.CourseID == id);
            if (model == null)
            {
                ViewBag.ErrorMessage = $"课程编号{id}的信息不存在，请重试";
                return View("NotFound");
            }
            await _courseAssignmentRepository.DeleteAsync(a => a.CourseID == model.CourseID);
            await _courseRepository.DeleteAsync(a => a.CourseID == id);
            return RedirectToAction(nameof(Index));
        }

        #region 修改课程学分

        public IActionResult UpdateCourseCredits()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCredits(int? multiplier)
        {
            if (multiplier != null)
            {
                ViewBag.RowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE School.Course SET Credits = Credits * {0}", parameters: multiplier);
            }
            return View();
        }

        #endregion 修改课程学分
    }
}