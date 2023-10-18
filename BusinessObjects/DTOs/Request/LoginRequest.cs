using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessObjects.DTOs.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required !")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required !")]
        [PasswordPropertyText]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
