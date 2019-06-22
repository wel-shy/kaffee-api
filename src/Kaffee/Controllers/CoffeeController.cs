using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Kaffee.Models;
using Kaffee.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace Kaffee.Controllers 
{
    /// <summary>
    /// Handle endpoints for the Coffee resource.
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly CoffeeService _coffeeService;

        /// <summary>
        /// Create a new instance of the CoffeeController.
        /// </summary>
        /// <param name="_coffeeService"></param>
        public CoffeeController(CoffeeService _coffeeService)
        {
            this._coffeeService = _coffeeService;
        }

        /// <summary>
        /// Get all coffees belonging to user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<List<Coffee>> Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            return _coffeeService.Get(id.Value);
        }

        /// <summary>
        /// Get a coffee by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the coffee item</response>
        /// <response code="404">If user does not have a coffee with matching id.</response>     
        [HttpGet("{id:length(24)}", Name = "GetCoffee")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Coffee> Get(string id)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var coffee = _coffeeService.GetWithId(id);

            if (coffee == null || !coffee.UserId.Equals(userId.Value))
            {
                return NotFound();
            }

            return coffee;
        }

        /// <summary>
        /// Create a coffee.
        /// </summary>
        /// <param name="coffee"></param>
        /// <returns></returns>
        /// <response code="201">Returns the new coffee item.</response>
        /// <response code="400">If parameters are wrong.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public ActionResult<Coffee> Create(Coffee coffee)
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            coffee.UserId = id.Value;
            coffee.CreatedAt = DateTime.Now;

            _coffeeService.Create(coffee);
            return CreatedAtRoute("GetCoffee", new { id = coffee.Id.ToString() }, coffee);
        }

        /// <summary>
        /// Update a coffee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coffeeIn"></param>
        /// <returns></returns>
        /// <response code="204">If the coffee was updated.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="404">If the user has no coffee with matching id.</response>
        [HttpPut("{id:length(24)}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult Update(string id, Coffee coffeeIn)
        {
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);
            var coffee = _coffeeService.GetWithId(id);

            if (coffee == null || !coffee.UserId.Equals(userId.Value))
            {
                return NotFound();
            }

            _coffeeService.Update(id, coffeeIn);
            return NoContent();
        }

        /// <summary>
        /// Delete a coffee.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">If the coffee was deleted.</response>
        /// <response code="400">If parameters are wrong.</response>
        /// <response code="404">If the user has no coffee with matching id.</response>
        [HttpDelete("{id:length(24)}")]
        [Produces("application/json")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Delete(string id)
        {
            var coffee = _coffeeService.GetWithId(id);
            var identity = User.Identity as ClaimsIdentity;
            var userId = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            if (coffee == null || !coffee.UserId.Equals(userId.Value))
            {
                return NotFound();
            }

            _coffeeService.Remove(coffee.Id);
            return NoContent();
        }
    }
}