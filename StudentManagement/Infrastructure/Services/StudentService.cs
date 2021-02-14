using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure.Interface;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Models;
using StudentManagement.ViewModels;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services
{
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student, int> _studentRepository;

        public StudentService(IRepository<Student, int> studentRepository)
        {
            this._studentRepository = studentRepository;
        }

        public async Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input)
        {
            var query = _studentRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText) || s.Email.Contains(input.FilterText));
            }
            var count = query.Count();
            var models = await query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount).AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Student>
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