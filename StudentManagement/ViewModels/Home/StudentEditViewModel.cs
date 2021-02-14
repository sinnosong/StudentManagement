using StudentManagement.Models;

namespace StudentManagement.ViewModels
{
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public string Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}