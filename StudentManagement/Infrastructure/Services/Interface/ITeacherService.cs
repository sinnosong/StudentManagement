using StudentManagement.Dtos;
using StudentManagement.Models;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Services.Interface
{
    public interface ITeacherService
    {
        Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input);
    }
}