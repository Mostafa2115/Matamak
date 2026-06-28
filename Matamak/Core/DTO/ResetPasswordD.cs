using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class ResetPasswordD
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } =string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }=string.Empty;
    }
}
