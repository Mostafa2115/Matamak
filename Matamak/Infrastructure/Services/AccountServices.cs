
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;
using System;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<AppUser> userManager;
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;

        public AccountServices(UserManager<AppUser> userManager , DataContext dataContext, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        public int GenerateActiveCode()
        {
            Random random = new Random();
            int activeCode = random.Next(100000, 999999);
            return activeCode;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]??string.Empty)
                ),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken
            );

            return principal;
        }

        void IAccountServices.sendActiveCode(string email, int code)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("Email cannot be null or empty.");
            }
            var masge = new MimeKit.MimeMessage();
            masge.From.Add(new MimeKit.MailboxAddress("Admin", "moazkrkor@gmail.com"));
            masge.To.Add(new MimeKit.MailboxAddress("User", email));
            masge.Subject = "Active Your Account";
            masge.Body = new MimeKit.TextPart("plain")
            {
                Text = $"Your Active Code Is : {code}"
            };

            var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("moazkrkor@gmail.com", "qevm frxv atwm purf");
            smtp.Send(masge);
            smtp.Disconnect(true);
        }
    }
}
