using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services.Interface
{
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;

        public CourseService(IRepository<Course, int> courseRepository)
        {
            this._courseRepository = courseRepository;
        }

        public async Task<PagedResultDto<Course>> GetPageinatedResult(GetCourseInput input)
        {
            var query = _courseRepository.GetAll();
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            var models = await query.Include(a => a.Department).AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Course>
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