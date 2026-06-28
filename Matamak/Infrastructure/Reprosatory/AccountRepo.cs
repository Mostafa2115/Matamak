using Core.IReprosatory;
using Infrastructure.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Reprosatory
{
    public class AccountRepo : IAccountRepo
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IDeliveryOrderRepo deliveryOrderRepo;
        private readonly ITakeAwayOrderRepo takeAwayOrderRepo;
        private readonly IAccountServices _accountServices;
        private readonly IConfiguration _configuration;

        public AccountRepo(UserManager<AppUser> userManager,IDeliveryOrderRepo deliveryOrderRepo, ITakeAwayOrderRepo takeAwayOrderRepo, IConfiguration configuration, IAccountServices accountServices)
        {   
                _accountServices = accountServices;
            _userManager = userManager;
            this.deliveryOrderRepo = deliveryOrderRepo;
            this.takeAwayOrderRepo = takeAwayOrderRepo;
            _configuration = configuration;
        }

        public async Task<AdminMV> GetAdminByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var adminMV = new AdminMV
            {
                username = user.UserName,
                Email = user.Email,
                Address = user.Address,
                FullName = user.FullName,
                Role = "Admin"
            };
            return adminMV;
        }

        public async Task<List<AdminMV>> GetAllAdmins()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var adminList = new List<AdminMV>();
            foreach (var user in admins)
            {
                var adminMV = new AdminMV
                {
                    username = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    FullName = user.FullName,
                    Role = "Admin"
                };
                adminList.Add(adminMV);
            }
            return adminList;
        }

        public async Task<List<CashirMV>> GetAllCashiers()
        {
            var cashiers = await _userManager.GetUsersInRoleAsync("Cashier");
            var cashierList = new List<CashirMV>();
            foreach (var user in cashiers)
            {
                var cashierMV = new CashirMV
                {
                    username = user.UserName,
                    Email = user.Email,
                    Address = user.Address,
                    FullName = user.FullName,
                    Role = "Cashier"
                };
                cashierList.Add(cashierMV);
            }
            return cashierList;
        }

        public async Task<List<CustmorMV>> GetAllCustomers()
        {
            var customers = await _userManager.GetUsersInRoleAsync("Customer");
            var customerList = new List<CustmorMV>();
            foreach (var user in customers)
            {
                var customerMV = new CustmorMV
                {
                    username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    FullName = user.FullName,
                    Role = "Customer"
                };
                customerList.Add(customerMV);
            }
            return customerList;
        }

        public async Task<CashirMV> GetCashierByUsername(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var cashierMV = new CashirMV
            {
                username = user.UserName,
                Email = user.Email,
                Address = user.Address,
                FullName = user.FullName,
                Role = "Cashier"
            };
            return cashierMV;
        }

        public async Task<CustmorMV> GetCustomerByUsername(string username)
        {
           var user = await  _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var customerMV = new CustmorMV
            {
                username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                FullName = user.FullName,
                deliveryOrders = new List<DeliveryOrderMV>(),
                takeawayOrders = new List<TakeAwayOrderMV>(),
                Role = "Customer"
            };
                var orders =  deliveryOrderRepo.GetDeliveryOrderByCustomerId(username);
                customerMV.deliveryOrders = orders;
                var takeawayOrders = takeAwayOrderRepo.GetTakeAwayOrderByCustomerName(username);
                customerMV.takeawayOrders = takeawayOrders;

            return customerMV;
        }

        public async Task ResendCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            int newcode = _accountServices.GenerateActiveCode();
            var dateTime2 = DateTime.Now.AddMinutes(15);
            _accountServices.sendActiveCode(email, newcode);

            user.ActiveCode = newcode;
            user.CodeExpiratioTime = dateTime2;
            await _userManager.UpdateAsync(user);
        }

        async Task IAccountRepo.ActiveAccount(string email, int code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            if(code==user.ActiveCode && user.CodeExpiratioTime>DateTime.Now)
            {
                user.IsValid = true;
                user.ActiveCode = null;
                user.CodeExpiratioTime = null;
                await _userManager.UpdateAsync(user);
            }
            else if(code!=user.ActiveCode && user.CodeExpiratioTime>DateTime.Now)
            {
                throw new Exception("Code is wrong");
            }
            else
            {
                int newcode = _accountServices.GenerateActiveCode();
                DateTime dateTime2 = DateTime.Now.AddMinutes(15);
                _accountServices.sendActiveCode(email, newcode);

                user.ActiveCode = newcode;
                user.CodeExpiratioTime = dateTime2;
                await _userManager.UpdateAsync(user);
                throw new Exception("Code Expired , New Code Sent To Your Email");
            }
        }

        async Task IAccountRepo.ChangePassword(string username, ChangePasswordD changePasswordD)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var result = await _userManager.CheckPasswordAsync(user, changePasswordD.oldPassword);
            if (!result)
            {
                throw new Exception("Old Password is wrong");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordD.oldPassword, changePasswordD.newPassword);
            if (!changePasswordResult.Succeeded)
            {
                throw new Exception("Failed to change password " + string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)));
            }
        }

        async Task IAccountRepo.CreateAccount(RegisterD registerD, string Role)
        {
            var user1 = await _userManager.FindByNameAsync(registerD.username);
            if (user1 !=null)
            {
                throw new Exception("Username already exists");
            }

            var user2 =await _userManager.FindByEmailAsync(registerD.email);
            if (user2 != null)
            {
                throw new Exception("Email already exists");
            }

            var activeCode = _accountServices.GenerateActiveCode();
           
            AppUser user = new AppUser();
            user.UserName = registerD.username;
            user.Email = registerD.email;
            user.PhoneNumber = registerD.phone;
            user.FullName = registerD.fullName;
            user.Address = registerD.address;
            user.ActiveCode = activeCode;
            user.CodeExpiratioTime=DateTime.Now.AddMinutes(15);
            user.IsValid = false;
            var result = await _userManager.CreateAsync(user, registerD.password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            var addRole = await _userManager.AddToRoleAsync(user, Role);
            if (!addRole.Succeeded)
            {
                throw new Exception("Failed to add role to user " + string.Join(", ", addRole.Errors.Select(e => e.Description)));
            }

            _accountServices.sendActiveCode(user.Email, activeCode);

        }

        async Task IAccountRepo.DeleteAccount(string username)
        {
            var user =await  _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to delete user " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        async Task IAccountRepo.EditAccount(string username, EditAccountD editAccountD)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            user.FullName = editAccountD.FullName;
            user.Address = editAccountD.Address;
            user.PhoneNumber = editAccountD.phonenumber;
            user.UserName = editAccountD.username;
          var result = await _userManager.UpdateAsync(user);
          if (!result.Succeeded)
          {
              throw new Exception("Failed to update user " + string.Join(", ", result.Errors.Select(e => e.Description)));
          }

        }

        async Task IAccountRepo.ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            int newcode = _accountServices.GenerateActiveCode();
            var dateTime2 = DateTime.Now.AddMinutes(15);
            _accountServices.sendActiveCode(email, newcode);

            user.CodeForgetPassword = newcode;
            user.ForgetPasswordCodeExpirationTime = dateTime2;
           var result = await _userManager.UpdateAsync(user);
           if (!result.Succeeded)
           {
               throw new Exception("Failed to update user " + string.Join(", ", result.Errors.Select(e => e.Description)));
           }
        }

        async Task<LoginTokenD> IAccountRepo.Login(LoginD loginD)
        {
           var user =await  _userManager.FindByEmailAsync(loginD.Email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            var result = await _userManager.CheckPasswordAsync(user, loginD.Password);
            if (!result)
            {
                throw new Exception("Password is wrong");
            }
            if (!user.IsValid)
            {
                throw new Exception("Account is not active");
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
            claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    signingCredentials:signingCredentials   ,
                    claims:claims,
                    expires:DateTime.Now.AddHours(8)
                );

            var refreshToken = _accountServices.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _userManager.UpdateAsync(user);

            LoginTokenD loginTokenD = new LoginTokenD();
            loginTokenD.LoginToken = new JwtSecurityTokenHandler().WriteToken(token);
            loginTokenD.RefreshToken = refreshToken;
            loginTokenD.TokenExpiration = DateTime.Now.AddHours(8);
            loginTokenD.RefreshTokenExpiration = DateTime.Now.AddDays(7);
            return loginTokenD;
        }

        async Task<LoginTokenD> IAccountRepo.RefreshToken(RefreshTokenD refreshTokenD)
        {
            var principal = _accountServices.GetPrincipalFromExpiredToken(refreshTokenD.Token);
            if (principal == null)
            {
                throw new Exception("Invalid Token");
            }
            var username = principal.Identity?.Name;
            if (username == null)
            {
                throw new Exception("Invalid Token");
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshTokenD.refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Invalid Token");
            }


            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName ?? ""));
            claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    signingCredentials: signingCredentials,
                    claims: claims,
                    expires: DateTime.Now.AddHours(8)
                );

            var newRefreshToken = _accountServices.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new LoginTokenD
            {
                LoginToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = newRefreshToken,
                TokenExpiration = DateTime.Now.AddHours(8),
                RefreshTokenExpiration = DateTime.Now.AddDays(7)
            };
        }

        
        async Task IAccountRepo.ResetPassword(string email, ResetPasswordD resetPasswordD)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            if (!user.CanResetPassword)
            {
                throw new Exception("You are not allowed to reset password");
            }
            var removePasswordResult = await _userManager.RemovePasswordAsync(user); 
            if (!removePasswordResult.Succeeded)
            {
                throw new Exception("Failed to remove old password " + string.Join(", ", removePasswordResult.Errors.Select(e => e.Description)));
            }
            var addPasswordResult = await _userManager.AddPasswordAsync(user, resetPasswordD.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                throw new Exception("Failed to add new password " + string.Join(", ", addPasswordResult.Errors.Select(e => e.Description)));
            }
            await _userManager.UpdateAsync(user);

        }

        async Task IAccountRepo.VerifyForgetPassword(string email, int code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            if (code == user.CodeForgetPassword && user.ForgetPasswordCodeExpirationTime > DateTime.Now)
            {
                user.CanResetPassword = true;
                user.CodeForgetPassword = null;
                user.ForgetPasswordCodeExpirationTime = null;
                await _userManager.UpdateAsync(user);
            }
            else if (code != user.CodeForgetPassword && user.ForgetPasswordCodeExpirationTime > DateTime.Now)
            {
                throw new Exception("Code is wrong");
            }
            else
            {
                int newcode = _accountServices.GenerateActiveCode();
                DateTime dateTime2 = DateTime.Now.AddMinutes(15);
                _accountServices.sendActiveCode(email, newcode);
                user.CodeForgetPassword = newcode;
                user.ForgetPasswordCodeExpirationTime = dateTime2;
                await _userManager.UpdateAsync(user);
                throw new Exception("Code Expired , New Code Sent To Your Email");
            }
        }
    }
}


