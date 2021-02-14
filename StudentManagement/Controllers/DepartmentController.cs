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
    public class DepartmentController : Controller
    {
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IDepartmentsService _departmentsService;
        private readonly IRepository<Teacher, int> _teacherRepository;
        private readonly AppDbContext _dbContext;

        public DepartmentController(IRepository<Department, int> departmentRepository, IDepartmentsService departmentsService
            , IRepository<Teacher, int> teacherRepository, AppDbContext dbContext)
        {
            this._departmentRepository = departmentRepository;
            this._departmentsService = departmentsService;
            this._teacherRepository = teacherRepository;
            this._dbContext = dbContext;
        }

        public async Task<IActionResult> Index(GetDepartmentInput input)
        {
            var models = await _departmentsService.GetPagedDepartmentsList(input);
            return View(models);
        }

        public IActionResult Create()
        {
            var dto = new DepartmentCreateViewModel { TeacherList = TeacherDropDownList() };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                Department model = new Department
                {
                    StartDate = input.StartDate,
                    DepartmentID = input.DepartmentID,
                    TeacherID = input.TeacherID,
                    Budget = input.Budget,
                    Name = input.Name
                };
                await _departmentRepository.InsertAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            string query = "SELECT * FROM dbo.Departments WHERE DepartmentID={0}";
            var model = await _dbContext.Departments.FromSqlRaw(query, id).Include(i => i.Administrator).AsNoTracking().FirstOrDefaultAsync();
            //var model = await _departmentRepository.GetAll().Include(i => i.Administrator).FirstOrDefaultAsync(i => i.DepartmentID == id);
            if (model == null)
            {
                ViewBag.Title = $"学院ID为{id}的信息不存在，请稍后重试";
                return View("NotFound");
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _departmentRepository.FirstOrDefaultAsync(i => i.DepartmentID == id);
            if (model == null)
            {
                ViewBag.Title = $"学院ID为{id}的信息不存在，请稍后重试";
                return View("NotFound");
            }
            await _departmentRepository.DeleteAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _departmentRepository.GetAll().Include(i => i.Administrator).AsNoTracking().FirstOrDefaultAsync(i => i.DepartmentID == id);
            if (model == null)
            {
                ViewBag.Title = $"学院ID为{id}的信息不存在，请稍后重试";
                return View("NotFound");
            }
            var teacherList = TeacherDropDownList();
            var dto = new DepartmentCreateViewModel
            {
                DepartmentID = model.DepartmentID,
                Name = model.Name,
                Budget = model.Budget,
                StartDate = model.StartDate,
                TeacherID = model.TeacherID,
                Administrator = model.Administrator,
                RowVersion = model.RowVersion,
                TeacherList = teacherList
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentCreateViewModel input)
        {
            if (ModelState.IsValid)
            {
                var model = await _departmentRepository.GetAll().Include(i => i.Administrator).FirstOrDefaultAsync(i => i.DepartmentID == input.DepartmentID);
                if (model == null)
                {
                    ViewBag.Title = $"学院ID为{input.DepartmentID}的信息不存在，请稍后重试";
                    return View("NotFound");
                }
                model.DepartmentID = input.DepartmentID;
                model.Name = input.Name;
                model.Budget = input.Budget;
                model.StartDate = input.StartDate;
                model.TeacherID = input.TeacherID;
                // 从数据库中获取Department实体中RowVersion属性，然后将input.RowVersion赋值到OriginalValue中
                _dbContext.Entry(model).Property("RowVersion").OriginalValue = input.RowVersion;
                try
                {
                    await _departmentRepository.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //触发异常后，获取异常实体
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    // 从数据库获取异常实体信息
                    var databaseEntity = exceptionEntry.GetDatabaseValues();
                    if (databaseEntity == null)
                    {
                        ModelState.AddModelError(string.Empty, "无法进行数据的修改。该学院信息已被其他人所删除！");
                    }
                    else
                    {
                        // 将异常实体中的错误信息精确到具体字段传递到视图中
                        var databaseValues = (Department)databaseEntity.ToObject();
                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"当前值：{databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"当前值：{databaseValues.Budget}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"当前值：{databaseValues.StartDate}");
                        }
                        if (databaseValues.TeacherID != clientValues.TeacherID)
                        {
                            var teacherEntity = await _teacherRepository.FirstOrDefaultAsync(i => i.Id == databaseValues.TeacherID);
                            ModelState.AddModelError("TeacherID", $"当前值：{teacherEntity?.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "您正在编辑的记录已经被其他用户所修改，编辑操作已经被取消，数据库当前值已显示到页面上，请再次保存，或返回列表");
                        input.RowVersion = databaseValues.RowVersion;
                        input.TeacherList = TeacherDropDownList();
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            return View(input);
        }

        /// <summary>
        /// 教师下拉列表
        /// </summary>
        /// <param name="selectedTeacher"></param>
        /// <returns></returns>
        private SelectList TeacherDropDownList(object selectedTeacher = null)
        {
            var models = _teacherRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "Id", "Name", selectedTeacher);
            return dtos;
        }
    }
}