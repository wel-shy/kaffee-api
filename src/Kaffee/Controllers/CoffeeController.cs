using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Kaffee.Models;
using Kaffee.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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
        private ILogger<CoffeeController> _logger;
        private readonly CoffeeService _coffeeService;
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// Create a new instance of the CoffeeController.
        /// </summary>
        /// <param name="_coffeeService">Service for persisting coffees.</param>
        /// <param name="_weatherService">Service for fetching weather.</param>
        /// <param name="_logger">Logger</param>
        public CoffeeController
        (
            CoffeeService _coffeeService,
            IWeatherService _weatherService,
            ILogger<CoffeeController> _logger
        )
        {
            this._coffeeService = _coffeeService;
            this._weatherService = _weatherService;
            this._logger = _logger;
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

            _logger.LogInformation(
                "CoffeeController - Getting all coffees for user {0}",
                id.Value
            );

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
            _logger.LogInformation(
                "CoffeeController - Getting coffee {0} for user {1}",
                id,
                userId.Value
            );

            var coffee = _coffeeService.GetWithId(id);

            if (coffee == null || !coffee.UserId.Equals(userId.Value))
            {
                _logger.LogWarning(
                    "CoffeeController - Could not find coffee {id} for user {0}",
                    id,
                    userId.Value
                );
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
        public async Task<ActionResult<Coffee>> Create(Coffee coffee)
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            _logger.LogInformation(
                "CoffeeController - Creating a coffee for user {0}",
                id.Value
            );

            coffee.UserId = id.Value;
            coffee.CreatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(coffee.Latitude) && !string.IsNullOrEmpty(coffee.Longitude))
            {
                var lat = float.Parse(coffee.Latitude);
                var lon = float.Parse(coffee.Longitude);

                if (!float.IsNaN(lat) && !float.IsNaN(lon))
                {
                    coffee.Weather = await _weatherService.GetWeather(lat, lon);
                }
            }

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

            _logger.LogInformation(
                "CoffeeController - Updating a coffee {0}",
                id
            );

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
            _logger.LogInformation(
                "CoffeeController - Deleting coffee {0}",
                id
            );

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