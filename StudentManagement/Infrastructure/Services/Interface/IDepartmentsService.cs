using StudentManagement.Dtos;
using StudentManagement.Models;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services.Interface
{
    public interface IDepartmentsService
    {
        Task<PagedResultDto<Department>> GetPagedDepartmentsList(GetDepartmentInput input);
    }
}