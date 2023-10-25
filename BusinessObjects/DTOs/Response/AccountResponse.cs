using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class AccountResponse
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; }
        public string? Avatar { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Gender { get; set; }
        public int Status { get; set; }
        public int RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Token { get; set; }
        public int? SpecializationId { get; set; }
        public string? RefreshToken { get; set; }
        public string? JwtId { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public SpecializationResponse Specialization { get; set; }
    }
}
