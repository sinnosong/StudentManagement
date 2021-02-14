using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Services.Interface;
using StudentManagement.Models;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository<Teacher, int> _teacherRepository;

        public TeacherService(IRepository<Teacher, int> teacherRepository)
        {
            this._teacherRepository = teacherRepository;
        }

        public async Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input)
        {
            var query = _teacherRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            var models = await query.Include(a => a.OfficeLocation).Include(a => a.CourseAssignments).ThenInclude(a => a.Course).ThenInclude(a => a.StudentCourses).ThenInclude(a => a.Student)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course).ThenInclude(i => i.Department).AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Teacher>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };
            return dtos;
        }
    }
}