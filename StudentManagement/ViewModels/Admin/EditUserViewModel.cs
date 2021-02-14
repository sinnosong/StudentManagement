using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StudentManagement.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<Claim>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(maximumLength: 30, MinimumLength = 10)]
        public string City { get; set; }

        [StringLength(maximumLength: 30, MinimumLength = 10)]
        public string Address { get; set; }

        public IList<Claim> Claims { get; set; }
        public IList<string> Roles { get; set; }
    }
}