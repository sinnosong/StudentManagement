using StudentManagement.Dtos;
using StudentManagement.Models;
using StudentManagement.ViewModels;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Interface
{
    public interface IStudentService
    {
        Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input);
    }
}