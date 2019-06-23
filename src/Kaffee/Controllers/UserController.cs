using System;
using Microsoft.AspNetCore.Mvc;
using Kaffee.Models;
using Kaffee.Services;
using Kaffee.Providers;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Kaffee.Controllers 
{
    /// <summary>
    /// Controller for the user resource.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private IConfiguration _configuration;

        /// <summary>
        /// Create a new instance of the user controller.
        /// </summary>
        /// <param name="_userService">User service.</param>
        /// <param name="_configuration">Server configuration.</param>
        public UserController(UserService _userService, IConfiguration _configuration)
        {
            this._userService = _userService;
            this._configuration = _configuration;
        }

        /// <summary>
        /// Get a user by id.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the user.</response>
        /// <response code="404">If the user can't be found.</response>
        [HttpGet(Name = "GetUser")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<User> Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var user = _userService.Get(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="201">Returns the user.</response>
        /// <response code="404">If the user can't be found.</response>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(201)]
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
            
            await _userService.Create(user);
            return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
        }

        /// <summary>
        /// Update a user.
        /// </summary>
        /// <param name="userIn"></param>
        /// <returns></returns>
        /// <response code="204">If the user was created.</response>
        /// <response code="404">If the user can't be found.</response>
        [HttpPut]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult Update(User userIn)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var user = _userService.Get(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(userId.Value, userIn);
            return NoContent();
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <returns></returns>
        /// <response code="204">If the user was created.</response>
        /// <response code="404">If the user can't be found.</response>
        [HttpDelete]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete()
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var user = _userService.Get(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.Remove(user.Id);
            return NoContent();
        }
    }
}