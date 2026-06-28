using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.DTO
{
    public class RegisterD
    {
        [Required]
        [MinLength(8)]
        public string username { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string fullName { get; set; }= string.Empty;
        [Required]
        public string address { get; set; } =string.Empty;
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string email { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string phone { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
        [Required]
        [Compare("password", ErrorMessage = "Not Match To Password")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
