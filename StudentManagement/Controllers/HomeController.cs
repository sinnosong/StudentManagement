using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure;
using StudentManagement.Infrastructure.Interface;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.Security;
using StudentManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Controllers
{
    [Allowanonymous]
    public class HomeController : Controller
    {
        private readonly IRepository<Student, int> _studentRepostiory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentService _studentService;
        private readonly AppDbContext _dbContext;
        private readonly IDataProtector protector;

        public HomeController(IRepository<Student, int> studentRepostiory, IWebHostEnvironment webHostEnvironment,
            IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, IStudentService studentService, AppDbContext dbContext)
        {
            _studentRepostiory = studentRepostiory;
            _webHostEnvironment = webHostEnvironment;
            this._studentService = studentService;
            this._dbContext = dbContext;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.StudentIdRouteValue);
        }

        public async Task<IActionResult> Index(GetStudentInput input)
        {
            var dtos = await _studentService.GetPaginatedResult(input);
            dtos.Data = dtos.Data.Select(s =>
            {
                s.EncryptedId = protector.Protect(s.Id.ToString());
                return s;
            }).ToList();
            return View(dtos);
        }

        public ViewResult Details(string id)
        {
            Student student = DecryptedStudent(id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生ID为{id}的信息不存在，请重试";
                return View("NotFound");
            }
            HomeDatailsViewModel homeDatailsViewModel = new HomeDatailsViewModel()
            {
                Student = student,
                PageTitle = "学生详情页"
            };
            homeDatailsViewModel.Student.EncryptedId = protector.Protect(student.Id.ToString());
            return View(homeDatailsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentCreateViewModel student)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = ProcessUploadedFile(student);
                Student newStudent = new Student()
                {
                    Name = student.Name,
                    Email = student.Email,
                    Major = student.Major,
                    EnrollmentDate = student.EnrollmentDate,
                    Photopath = uniqueFileName
                };
                _studentRepostiory.Insert(newStudent);
                var encryptedId = protector.Protect(newStudent.Id.ToString());
                return RedirectToAction("Details", new { id = encryptedId });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(string id)
        {
            Student student = DecryptedStudent(id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"学生ID为{id}的信息不存在，请重试";
                return View("NotFound");
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel()
            {
                Id = id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                EnrollmentDate = student.EnrollmentDate,
                ExistingPhotoPath = student.Photopath
            };
            return View(studentEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Student student = DecryptedStudent(model.Id);
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                student.EnrollmentDate = model.EnrollmentDate;
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    student.Photopath = ProcessUploadedFile(model);
                }
                _studentRepostiory.Update(student);
                return RedirectToAction("index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentRepostiory.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的学生信息";
                return View("NotFound");
            }
            await _studentRepostiory.DeleteAsync(s => s.Id == id);
            return RedirectToAction("index");
        }

        private string ProcessUploadedFile(StudentCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public async Task<IActionResult> About()
        {
            List<EnrollmentDateGroupDto> groups = new List<EnrollmentDateGroupDto>();
            // 获取数据库链接
            var conn = _dbContext.Database.GetDbConnection();
            try
            {
                await conn.OpenAsync(); // 打开数据库链接
                using (var command = conn.CreateCommand())
                {
                    string query = "SELECT EnrollmentDate,COUNT(*) AS StudentCount FROM Person Where Discriminator='Student' Group by EnrollmentDate";
                    command.CommandText = query;
                    DbDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new EnrollmentDateGroupDto
                            {
                                EnrollmentDate = reader.GetDateTime(0),
                                StudentCount = reader.GetInt32(1)
                            };
                            groups.Add(row);
                        }
                    }
                    reader.Dispose();
                }
            }
            finally { conn.Close(); }
            return View(groups);
        }

        //Linq 版本
        //public async Task<IActionResult> About()
        //{
        //    var data = from student in _studentRepostiory.GetAll()
        //               group student by student.EnrollmentDate into dateGroup
        //               select new EnrollmentDateGroupDto
        //               { EnrollmentDate = dateGroup.Key, StudentCount = dateGroup.Count() };
        //    var dtos = await data.AsNoTracking().ToListAsync();
        //    return View(dtos);
        //}

        private Student DecryptedStudent(string id)
        {
            string decryptedId = protector.Unprotect(id);
            int decryptedStudentId = Convert.ToInt32(decryptedId);
            Student student = _studentRepostiory.FirstOrDefault(s => s.Id == decryptedStudentId);
            return student;
        }
    }
}