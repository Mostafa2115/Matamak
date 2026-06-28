using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO
{
    public class LoginTokenD
    {
        public string LoginToken { get; set; }= string.Empty;
        public string RefreshToken { get; set; }=string.Empty;
        public DateTime TokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
