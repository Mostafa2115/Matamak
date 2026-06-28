using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class ChangePasswordD
    {
        [Required]
        [DataType(DataType.Password)]
        public string oldPassword { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string newPassword { get; set; }=string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("newPassword", ErrorMessage = "Not Match To New Password")]
        public string confirmPassword { get; set; }= string.Empty;
    }
}
