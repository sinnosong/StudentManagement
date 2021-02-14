using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public abstract class Person
    {
        public int Id { get; set; }
        public string Email { get; set; }

        [Required]
        [Display(Name = "姓名")]
        [StringLength(50)]
        public string Name { get; set; }
    }
}