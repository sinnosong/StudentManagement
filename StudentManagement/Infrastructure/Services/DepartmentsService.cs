using Microsoft.EntityFrameworkCore;
using StudentManagement.Dtos;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Services.Interface;
using StudentManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services
{
    public class DepartmentsService : IDepartmentsService
    {
        private readonly IRepository<Department, int> _departmentRepository;

        public DepartmentsService(IRepository<Department, int> departmentRepository)
        {
            this._departmentRepository = departmentRepository;
        }

        public async Task<PagedResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input)
        {
            var query = _departmentRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }
            var count = query.Count();
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            List<Department> models = await query.Include(i => i.Administrator).AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Department>
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