using Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IReprosatory
{
    public interface IAccountRepo
    {
        public Task CreateAccount(RegisterD registerD,string Role);
        public Task<LoginTokenD> Login(LoginD loginD);
        public Task<LoginTokenD> RefreshToken(RefreshTokenD refreshTokenD);
        public Task DeleteAccount(string username);
        public Task EditAccount(string username, EditAccountD editAccountD);
        public Task ActiveAccount(string email, int code);
        public Task ResendCode(string email);
        public Task ChangePassword(string username, ChangePasswordD changePasswordD);
        public Task ForgetPassword(string email);
        public Task VerifyForgetPassword(string email, int code);
        public Task ResetPassword(string email, ResetPasswordD resetPasswordD);
        public Task<List<CustmorMV>> GetAllCustomers();
        public Task<CustmorMV> GetCustomerByUsername(string username);
        public Task<List<CashirMV>> GetAllCashiers();
        public Task<CashirMV> GetCashierByUsername(string username);
        public Task<List<AdminMV>> GetAllAdmins();
        public Task<AdminMV> GetAdminByUsername(string username);
    }
}
