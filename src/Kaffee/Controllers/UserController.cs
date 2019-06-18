using System;
using Microsoft.AspNetCore.Mvc;
using Kaffee.Models;
using Kaffee.Services;
using Kaffee.Providers;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Kaffee.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private IConfiguration _configuration;

        public UserController(UserService _userService, IConfiguration _configuration)
        {
            this._userService = _userService;
            this._configuration = _configuration;
        }

        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public ActionResult<User> Get(string id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            if (await _userService.GetWithEmail(user.Email) != null) 
            {
                return Unauthorized("Email already exists.");
            }
            var salt = HashProvider.GetSalt();
            user.IV = salt;
            user.Password = HashProvider.GetHash(user.Password, salt);
            user.CreatedAt = DateTime.Now;

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            user.RefreshToken = TokenProvider.GetToken
                (
                    user, 
                    _configuration["SecurityKey"], 
                    DateTime.Now.AddYears(100), 
                    claims
                );
            
            _userService.Create(user);
            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User userIn)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(id, userIn);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Remove(user.Id);
            return NoContent();
        }
    }
}