using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Kaffee.Models;
using Kaffee.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Kaffee.Policies;

namespace Kaffee.Controllers 
{
    /// <summary>
    /// Handle endpoints for the Coffee resource.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly LeaderboadService _leaderboardService;
        private readonly ILogger<CoffeeController> _logger;

        /// <summary>
        /// Create a new instance of the CoffeeController.
        /// </summary>
        /// <param name="_leaderboardService">Service for persisting coffees.</param>
        /// <param name="_logger">Logger</param>
        public LeaderboardController(
            LeaderboadService _leaderboardService,
            ILogger<CoffeeController> _logger
        )
        {
            this._leaderboardService = _leaderboardService;
            this._logger = _logger;
        }

        /// <summary>
        /// Get all the users leaderboards.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<List<Leaderboard>> Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            return _leaderboardService.GetUsersLeaderboards(id.Value);
        }

        /// <summary>
        /// Get a board by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the board item</response>
        /// <response code="401">If the user is not a member of the board.</response>
        /// <response code="404">If user does not have a board with matching id.</response>     
        [HttpGet("{id:length(24)}", Name = "GetBoard")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public ActionResult<Leaderboard> Get(string id)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null)
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanRead(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            return leaderboard;
        }

        /// <summary>
        /// Create a leaderboard.
        /// </summary>
        /// <param name="leaderboard"></param>
        /// <returns></returns>
        /// <response code="201">Returns the new leaderboard item.</response>
        /// <response code="400">If parameters are wrong.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Leaderboard>> Create(Leaderboard leaderboard)
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            leaderboard.Administrators = new string[] { id.Value };
            leaderboard.CreatedAt = DateTime.Now;

            await _leaderboardService.Create(leaderboard);
            return CreatedAtRoute("GetBoard", new { id = leaderboard.Id.ToString() }, leaderboard);
        }

        /// <summary>
        /// Update a board
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boardIn"></param>
        /// <returns></returns>
        /// <response code="204">If the board was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the users does not have permission.</response>
        /// <response code="404">If the user has no board with matching id.</response>
        [HttpPut("{id:length(24)}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, Leaderboard boardIn)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null) 
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(boardIn, userId.Value))
            {
                return Unauthorized();
            }

            await _leaderboardService.Update(id, boardIn);
            return NoContent();
        }

        /// <summary>
        /// Add a member to a board.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <response code="204">If the board was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the users does not have permission.</response>
        /// <response code="404">If the user has no board with matching id or user cannot be found.</response>
        [HttpPut("{id:length(24)}/members/add")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddMember(string id, string email)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null) 
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            try
            {
                await _leaderboardService.AddMember(leaderboard, email);
            }
            catch (System.Exception e)
            {
                if (e.Message.Contains("User not found."))
                {
                    return NotFound();
                }
                throw e;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove a member from a board.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <response code="204">If the board was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the users does not have permission.</response>
        /// <response code="404">If the user has no board with matching id or user cannot be found.</response>
        [HttpDelete("{id:length(24)}/members/remove/{email}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveMember(string id, string email)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null) 
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            try
            {
                await _leaderboardService.RemoveMember(leaderboard, email);
            }
            catch (System.Exception e)
            {
                if (e.Message.Contains("User not found."))
                {
                    return NotFound();
                }
                throw e;
            }

            return NoContent();
        }

        /// <summary>
        /// Add an admin to a board.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <response code="204">If the board was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the users does not have permission.</response>
        /// <response code="404">If the user has no board with matching id or user cannot be found.</response>
        [HttpPut("{id:length(24)}/admins/add")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddAdmin(string id, string email)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null) 
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            try
            {
                await _leaderboardService.AddAdmin(leaderboard, email);
            }
            catch (System.Exception e)
            {
                if (e.Message.Contains("User not found."))
                {
                    return NotFound();
                }
                throw e;
            }

            return NoContent();
        }

        /// <summary>
        /// Remove an admin from a board.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <response code="204">If the board was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the users does not have permission.</response>
        /// <response code="404">If the user has no board with matching id or user cannot be found.</response>
        [HttpDelete("{id:length(24)}/admins/remove/{email}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveAdmin(string id, string email)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var leaderboard = _leaderboardService.Get(id);

            if (leaderboard == null) 
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            try
            {
                await _leaderboardService.RemoveMember(leaderboard, email);
            }
            catch (System.Exception e)
            {
                if (e.Message.Contains("User not found."))
                {
                    return NotFound();
                }
                throw e;
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a leaderboard.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">If the board was deleted.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="401">If the user does not have permission.</response>
        /// <response code="404">If the user has no board with matching id.</response>
        [HttpDelete("{id:length(24)}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Delete(string id)
        {
            var leaderboard = _leaderboardService.Get(id);
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            if (leaderboard == null)
            {
                return NotFound();
            }

            if (!LeaderboardPolicies.CanEdit(leaderboard, userId.Value))
            {
                return Unauthorized();
            }

            _leaderboardService.Remove(leaderboard.Id);
            return NoContent();
        }
    }
}