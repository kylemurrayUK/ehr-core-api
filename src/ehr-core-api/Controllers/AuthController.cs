using Microsoft.AspNetCore.Mvc;
using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Models;
using EHRCoreAPI.Services;

namespace EHRCoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest)
        {
            var loginRequestResult = await _authService.Login(loginRequest);

            if(loginRequestResult.LoginOutcome == LoginOutcome.InvalidCredentials)
            {
                return Unauthorized();
            }
            else if(loginRequestResult.LoginOutcome == LoginOutcome.AccountInactive)
            {
                return StatusCode(403, "Account is not active");
            }
            else
            {
                var loginResponse = new LoginResponseDTO
                {
                    Token = loginRequestResult.Token!,
                    ExpiresAt = loginRequestResult.ExpiresAt!.Value
                };
                return Ok(loginResponse);
            }

        }
    }
}