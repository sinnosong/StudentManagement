using StudentManagement.Dtos;
using StudentManagement.Models;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services.Interface
{
    public interface ICourseService
    {
        Task<PagedResultDto<Course>> GetPageinatedResult(GetCourseInput input);
    }
}