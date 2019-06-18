using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc;
using Kaffee.Services;
using Kaffee.Models;
using Kaffee.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Kaffee.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TokenController : ControllerBase
    {
        private UserService _userService;
        private IConfiguration _configuration;

        public TokenController(IConfiguration _configuration, UserService _userService)
        {
            this._userService = _userService;
            this._configuration = _configuration;
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestTokenWithRefresh(RefreshTokenRequest tokenRequest)
        {
            var user = await _userService.GetWithToken(tokenRequest.RefreshToken);
            if (user == null) 
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            return Ok(new
            {
                token = TokenProvider.GetToken
                    (
                        user, 
                        _configuration["SecurityKey"], 
                        DateTime.Now.AddDays(1), 
                        claims
                    )
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestTokenWithCredentials(LoginRequest loginRequest)
        {
            var user = await _userService.GetWithEmail(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized();
            } 

            var match = HashProvider.GetHash(loginRequest.Password, user.IV);
            if (!user.Password.Equals(HashProvider.GetHash(loginRequest.Password, user.IV))) 
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            return Ok(new
            {
                token = TokenProvider.GetToken
                    (
                        user, 
                        _configuration["SecurityKey"], 
                        DateTime.Now.AddDays(1), 
                        claims
                    ),
                refreshToken = user.RefreshToken
            });
        }
    }
}