using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Kaffee.Models;
using Kaffee.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace Kaffee.Controllers 
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly CoffeeService _coffeeService;

        public CoffeeController(CoffeeService _coffeeService)
        {
            this._coffeeService = _coffeeService;
        }

        [HttpGet]
        public ActionResult<List<Coffee>> Get()
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            return _coffeeService.Get(id.Value);
        }

        [HttpGet("{id:length(24)}", Name = "GetCoffee")]
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

        [HttpPost]
        public ActionResult<Coffee> Create(Coffee coffee)
        {
            var identity = User.Identity as ClaimsIdentity;
            var id = identity.Claims.First((c) => c.Type == ClaimTypes.PrimarySid);

            coffee.UserId = id.Value;
            coffee.CreatedAt = DateTime.Now;

            _coffeeService.Create(coffee);
            return CreatedAtRoute("GetCoffee", new { id = coffee.Id.ToString() }, coffee);
        }

        [HttpPut("{id:length(24)}")]
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

        [HttpDelete("{id:length(24)}")]
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