using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kaffee.Models;
using Kaffee.Services;
using System;

namespace Kaffee.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly CoffeeService _coffeeService;

        public CoffeeController(CoffeeService _coffeeService)
        {
            this._coffeeService = _coffeeService;
        }

        [HttpGet]
        public ActionResult<List<Coffee>> Get() =>
            _coffeeService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCoffee")]
        public ActionResult<Coffee> Get(string id)
        {
            var coffee = _coffeeService.Get(id);

            if (coffee == null)
            {
                return NotFound();
            }

            return coffee;
        }

        [HttpPost]
        public ActionResult<Coffee> Create(Coffee coffee)
        {
            _coffeeService.Create(coffee);

            return CreatedAtRoute("GetCoffee", new { id = coffee.Id.ToString() }, coffee);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Coffee coffeeIn)
        {
            var coffee = _coffeeService.Get(id);

            if (coffee == null)
            {
                return NotFound();
            }

            _coffeeService.Update(id, coffeeIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var coffee = _coffeeService.Get(id);

            if (coffee == null)
            {
                return NotFound();
            }

            _coffeeService.Remove(coffee.Id);

            return NoContent();
        }
    }
}