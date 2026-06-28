using Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.IServices
{
    public interface IAccountServices
    {
        
        public int GenerateActiveCode();
        public void sendActiveCode(string email, int code);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    }
}
