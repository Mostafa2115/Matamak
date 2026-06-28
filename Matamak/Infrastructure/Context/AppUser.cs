using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Context
{
    public class AppUser:IdentityUser
    {
        
        public string FullName { get; set; }= string.Empty;
        public string Address { get; set; }= string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsValid { get; set; }
        public int? ActiveCode { get; set; }
        public int? CodeForgetPassword { get; set; }
        public bool CanResetPassword { get; set; } = false;
        public DateTime? ForgetPasswordCodeExpirationTime { get; set; }
        public DateTime? CodeExpiratioTime { get; set; }
    }
}
