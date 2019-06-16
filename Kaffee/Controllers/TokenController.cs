using System.Threading.Tasks;
using System.Security.Claims;
using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Kaffee.Services;
using Kaffee.Models;
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "kaffee.dwelsh.uk",
                audience: "kaffee.dwelsh.uk",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestTokenWithCredentials(LoginRequest loginRequest)
        {
            var user = await _userService.GetWithEmail(loginRequest.Email);
            if (user == null || !user.Password.Equals(loginRequest.Password)) 
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "kaffee.dwelsh.uk",
                audience: "kaffee.dwelsh.uk",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}