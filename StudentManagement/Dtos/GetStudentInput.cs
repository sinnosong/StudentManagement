using StudentManagement.Dtos;

namespace StudentManagement.ViewModels
{
    public class GetStudentInput : PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";
        }
    }
}