using Core.DTO;
using Core.IReprosatory;
using Core.IServices;
using Infrastructure.Context;
using Infrastructure.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Resturant.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v1/auth")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IAccountRepo accountRepo;
        private readonly DataContext dataContext;
        private readonly IConfiguration configuration;
        private readonly IAccountServices accountServices;

        public AccountController(UserManager<AppUser> userManager,IAccountRepo accountRepo, DataContext dataContext, IConfiguration configuration, IAccountServices accountServices)
        {
            this.userManager = userManager;
            this.accountRepo = accountRepo;
            this.dataContext = dataContext;
            this.configuration = configuration;
            this.accountServices = accountServices;
        }

        //Register--------------------------------------------------------------------------------

        [HttpPost]
        [Route("Customerregister")]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterD registerD )
        { 
            
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await accountRepo.CreateAccount(registerD, "Customer");
                return Ok("Account Created Successfully , Please Check Your Email To Active Your Account ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles ="Admin")]
        [HttpPost("manger&cashierRegister")]
        public async Task<IActionResult> registerAsync(RegisterD registerD, string role)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await accountRepo.CreateAccount(registerD, role);
                return Ok("Account Created Successfully , Please Check Your Email To Active Your Account ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Active Account-------------------------------------------------------------------
        [HttpPost("activeAccount/{email}")]
        public async Task<IActionResult> ActiveAccount([FromRoute] string email, [FromBody] int code)
        {
            try 
            {
                await accountRepo.ActiveAccount(email, code);
                return Ok("Account Activated Successfully");

            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

        }

        //Login--------------------------------------------------------------------------------

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginD loginD)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
               var result = await accountRepo.Login(loginD);
                return Ok(result);
            } 
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

           
        }


        //Refresh Token--------------------------------------------------------------------------------
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenD tokenD)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await accountRepo.RefreshToken(tokenD);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
             
        }

        [Authorize]
        [HttpPut("EditAccount/{username}")]
        public async Task<IActionResult> UpdateAccount([FromRoute] string username, [FromBody] EditAccountD updateAccountD)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                 await accountRepo.EditAccount(username, updateAccountD);
                return Ok("Account Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPut("ChangePassword/{username}")]
        public async Task<IActionResult> ChangePassword([FromRoute] string username, [FromBody] ChangePasswordD changePasswordD)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await accountRepo.ChangePassword(username, changePasswordD);
                return Ok("Password Changed Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpDelete("DeleteMyAccount/{username}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] string username)
        {
            try
            {
                await accountRepo.DeleteAccount(username);
                return Ok("Account Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("DeleteAnyAccount/{username}")]
        public async Task<IActionResult> DeleteAnyAccount([FromRoute] string username)
        {
            try
            {
                await accountRepo.DeleteAccount(username);
                return Ok("Account Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await accountRepo.ForgetPassword(email);
                return Ok("You will receive an email with Code to reset your password.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("VerifyForgetPasswordCode/{email}")]
        public async Task<IActionResult> VerifyCode([FromRoute] string email, [FromBody] int code)
        {
            try 
            {
                 await accountRepo.VerifyForgetPassword(email, code);
                return Ok("Code Verified Successfully , You Can Now Reset Your Password");
            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("ResetPassword/{email}")]
        public async Task<IActionResult> ResetPassword([FromRoute] string email, [FromBody] ResetPasswordD newPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await accountRepo.ResetPassword(email, newPassword);
                return Ok("Password Reset Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("GetAllAdmins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                var admins = await accountRepo.GetAllAdmins();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAdminByUsername/{username}")]
        public async Task<IActionResult> GetAdminByUsername([FromRoute] string username)
        {
            try
            {
                var admin = await accountRepo.GetAdminByUsername(username);
                return Ok(admin);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllCashiers")]
            public async Task<IActionResult> GetAllCashiers()
            {
                try
                {
                    var cashiers = await accountRepo.GetAllCashiers();
                    return Ok(cashiers);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetCashierByUsername/{username}")]
        public async Task<IActionResult> GetCashierByUsername([FromRoute] string username)
        {
            try
            {
                var cashier = await accountRepo.GetCashierByUsername(username);
                return Ok(cashier);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await accountRepo.GetAllCustomers();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpGet("GetCustomerByUsername/{username}")]
        public async Task<IActionResult> GetCustomerByUsername([FromRoute] string username)
        {
            try
            {
                var customer = await accountRepo.GetCustomerByUsername(username);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("User not found.");
            }

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;
            await userManager.UpdateAsync(user);
            return Ok("Logged out successfully.");
        }

        [Authorize]
        [HttpGet("order-history/{username}")]
        public IActionResult GetOrderHistory([FromRoute] string username)
        {
            var deliveryOrders = dataContext.DeliveryOrders
                .Where(o => o.CustomerUsername == username || o.CustomerName == username)
                .Select(o => new
                {
                    Type = "Delivery",
                    o.Id,
                    o.orderNumber,
                    o.OrderDate,
                    o.TotalPrice,
                    o.Status
                })
                .ToList();

            var takeawayOrders = dataContext.TakeawayOrders
                .Where(o => o.CustomerName == username)
                .Select(o => new
                {
                    Type = "Takeaway",
                    o.Id,
                    o.orderNumber,
                    o.OrderDate,
                    o.TotalPrice,
                    o.Status
                })
                .ToList();

            return Ok(deliveryOrders.Cast<object>().Concat(takeawayOrders.Cast<object>()));
        }

        
    }
}
