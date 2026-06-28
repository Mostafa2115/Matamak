using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public class RefreshTokenD
    {
        [Required]
        public string Token { get; set; }=string.Empty;
        [Required]
        public string refreshToken { get; set; }=string.Empty;
    }
}
