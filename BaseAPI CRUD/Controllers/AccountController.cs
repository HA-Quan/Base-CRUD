using Microsoft.AspNetCore.Mvc;
using Services.Model;
using Services.Service;

namespace BaseAPI_CRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("SingUp")]
        public IActionResult AddAccount([FromBody] SingUpModel acc)
        {
            try
            {
                var result = _accountService.AddAccount(acc);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new ApiReponse
                {
                    Success = false,
                    Message = "Something is wrongs!"
                });
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginModel loginModel)
        {
            var acc = _accountService.GetAccount(loginModel);
            if (acc == null)
            {
                return BadRequest(new ApiReponse
                {
                    Success = false,
                    Message  = "Invalid Username or Password"
                });
            }
            else
            {
                var result = _accountService.GenerateToken(acc);
                return Ok(new ApiReponse
                {
                    Success = true,
                    Message = "Create Token Succes",
                    Data = result
                });
            }

        }

        [HttpPost("RenewToken")]
        public IActionResult RenewToken(TokenModel tokenModel)
        {
            var result = _accountService.RenewToken(tokenModel);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
