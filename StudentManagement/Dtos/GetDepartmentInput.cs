namespace StudentManagement.Dtos
{
    public class GetDepartmentInput : PagedSortedAndFilterInput
    {
        public GetDepartmentInput()
        {
            Sorting = "Name";
            MaxResultCount = 3;
        }
    }
}