using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class AccountRequest
    {
        [StringAttribute]
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Invalid Name")]
        public string Name { get; set; } = null!;
        [StringAttribute]
        [Required(ErrorMessage = "Email is required !")]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone is invalid")]
        [StringAttribute]
        public string? Phone { get; set; }
        [Required]
        [StringAttribute]
        public string Code { get; set; } = null!;
        [StringAttribute]
        public string? Gender { get; set; }
        [StringAttribute]
        public string SpecializationCode { get; set; }
    }
}
