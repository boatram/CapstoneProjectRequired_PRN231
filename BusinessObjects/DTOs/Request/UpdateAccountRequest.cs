using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class UpdateAccountRequest
    {
        public int Id { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Invalid Name")]
        public string Name { get; set; } = null!;
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Phone is invalid")]
        public string Phone { get; set; }
        public string? OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string? ConfirmNewPassword { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
    }
}
