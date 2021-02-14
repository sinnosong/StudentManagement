using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Services.Interface;
using StudentManagement.Models;
using StudentManagement.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<OfficeLocation, int> _officeLocationRepository;
        private readonly IRepository<CourseAssignment, int> _courseAssignmentRepository;

        public TeacherController(ITeacherService teacherService, IRepository<Teacher, int> teacherRepository,
            IRepository<Course, int> courseRepository, IRepository<OfficeLocation, int> officeLocationRepository,
            IRepository<CourseAssignment, int> courseAssignmentRepository)
        {
            this._teacherService = teacherService;
            this._teacherRepository = teacherRepository;
            this._courseRepository = courseRepository;
            this._officeLocationRepository = officeLocationRepository;
            this._courseAssignmentRepository = courseAssignmentRepository;
        }

        // GET: TeacherController
        public async Task<IActionResult> Index(GetTeacherInput input)
        {
            var models = await _teacherService.GetPagedTeacherList(input);
            var dto = new TeacherListViewModel();
            if (input.Id != null)
            {
                var teacher = models.Data.FirstOrDefault(a => a.Id == input.Id.Value);
                if (teacher != null)
                {
                    dto.Courses = teacher.CourseAssignments.Select(a => a.Course).ToList();
                }
                dto.SelectedId = input.Id.Value;
            }
            if (input.CourseId.HasValue)
            {
                var course = dto.Courses.FirstOrDefault(a => a.CourseID == input.CourseId.Value);
                if (course != null)
                {
                    dto.StudentCourses = course.StudentCourses.ToList();
                }
                dto.SelectedCourseId = input.CourseId.Value;
            }
            dto.Teachers = models;
            return View(dto);
        }

        // GET: TeacherController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TeacherController/Create
        public IActionResult Create()
        {
            var allCourses = _courseRepository.GetAllList();
            var viewModel = new List<AssignedCourseViewModel>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseViewModel
                {
                    CourseID = course.CourseID,
                    Titile = course.Title,
                    IsSelected = false
                });
            }
            var dto = new TeacherCreateViewModel
            {
                AssignedCourses = viewModel
            };
            return View(dto);
        }

        // POST: TeacherController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var teacher = new Teacher
                {
                    HireDate = input.HireDate,
                    Name = input.Name,
                    OfficeLocation = input.OfficeLocation,
                    CourseAssignments = new List<CourseAssignment>()
                };
                // 获取用户选中的课程信息
                var courses = input.AssignedCourses.Where(i => i.IsSelected == true).ToList();
                foreach (var item in courses)
                {
                    //叫选中的课程添加到导航属性中
                    teacher.CourseAssignments.Add(new CourseAssignment
                    {
                        CourseID = item.CourseID,
                        TeacherID = teacher.Id
                    });
                }
                await _teacherRepository.InsertAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        // GET: TeacherController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await _teacherRepository.GetAll().Include(a => a.OfficeLocation)
                .Include(a => a.CourseAssignments).ThenInclude(a => a.Course).AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (model == null)
            {
                ViewBag.Title = $"教师信息ID为{id}的信息不存在，请重试";
                return View("NotFound");
            }
            var dto = new TeacherCreateViewModel
            {
                Name = model.Name,
                Id = model.Id,
                HireDate = model.HireDate,
                OfficeLocation = model.OfficeLocation
            };
            var assignedCourses = AssignedCourseDropDownList(model);
            dto.AssignedCourses = assignedCourses;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeacherCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var teacher = await _teacherRepository.GetAll().Include(i => i.OfficeLocation).Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                    .FirstOrDefaultAsync(m => m.Id == input.Id);
                if (teacher == null)
                {
                    ViewBag.Title = $"教师信息ID为{input.Id}的信息不存在，请重试";
                    return View("NotFound");
                }
                teacher.HireDate = input.HireDate;
                teacher.Name = input.Name;
                teacher.OfficeLocation = input.OfficeLocation;
                teacher.CourseAssignments = new List<CourseAssignment>();
                //从视图中获取选中的课程信息
                var courses = input.AssignedCourses.Where(a => a.IsSelected == true).ToList();
                foreach (var item in courses)
                {
                    teacher.CourseAssignments.Add(new CourseAssignment
                    {
                        CourseID = item.CourseID,
                        TeacherID = teacher.Id
                    });
                }
                await _teacherRepository.UpdateAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        /// <summary>
        /// 判断课程列表是否被选中
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns></returns>
        private List<AssignedCourseViewModel> AssignedCourseDropDownList(Teacher teacher)
        {
            //获取课程列表
            var allCoureses = _courseRepository.GetAllList();
            //获取教师当前教授的课程
            var teacherCourses = new HashSet<int>(teacher.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseViewModel>();
            foreach (var course in allCoureses)
            {
                viewModel.Add(new AssignedCourseViewModel
                {
                    CourseID = course.CourseID,
                    Titile = course.Title,
                    IsSelected = teacherCourses.Contains(course.CourseID)
                });
            }
            return viewModel;
        }

        // POST: TeacherController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _teacherRepository.FirstOrDefaultAsync(a => a.Id == id);
            if (model == null)
            {
                ViewBag.Title = $"教师信息ID为{id}的信息不存在，请重试";
                return View("NotFound");
            }
            await _officeLocationRepository.DeleteAsync(a => a.TeacherId == model.Id);
            await _courseAssignmentRepository.DeleteAsync(a => a.TeacherID == model.Id);
            await _teacherRepository.DeleteAsync(a => a.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
}