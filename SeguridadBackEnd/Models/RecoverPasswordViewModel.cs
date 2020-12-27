using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeguridadBackEnd.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Current password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string newPassword { get; set; }

        [Display(Name = "Current password")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} field must contain between {2} and {1} characters.")]
        public string confirm { get; set; }
    }
}
