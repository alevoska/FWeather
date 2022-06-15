using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FWeather;
using FWeather.Models;


namespace FWeather.Controllers
{
    [ApiController]
    [Route("weather")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherContext _context;

        public WeatherForecastController(WeatherContext context)
        {
            _context = context;
        }

        [HttpGet("regenerate/{locationId}")]
        public async Task<IActionResult> Regenerate(int locationId)
        {
            Random rnd = new Random();
            Func<double, double> TemperatureGenerator = x => 20 * Math.Sin((2 * Math.PI * x / 366) - (2 * Math.PI / 3));

            await _context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Items WHERE locationId = {locationId}");

            var trx = await _context.Database.BeginTransactionAsync();
            DateTime from = new DateTime(2022, 1, 1);
            DateTime to = new DateTime(2023, 1, 1);

            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                var temperature = TemperatureGenerator(day.DayOfYear) + rnd.Next(-2, 8);
                var dateFormat = day.ToString("yyyy-MM-dd");

                _context.Items.Add(new Item
                {
                    Date = dateFormat,
                    LocationId = locationId,
                    Temperature = Math.Round(temperature * 2, MidpointRounding.AwayFromZero) / 2,
                    Precipitation = Math.Round(Math.Max(rnd.NextDouble() * 2 - 1, 0.0) * 3, 1),
                    WindDirection = rnd.Next(360),
                    WindStrength = rnd.Next(0, 7)
                });
            }
            await _context.SaveChangesAsync();
            await trx.CommitAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> ListLocations()
        {
            return await _context.Locations.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetLocation(int id)
        {
            return await _context.Items
                .Where(i => i.LocationId == id)
                .OrderBy(i => i.Date)
                .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(int id, Location location)
        {
            if (id != location.LocationId)
            {
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.LocationId }, location);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }

        [HttpGet("{locationId}/{date}")]
        public async Task<ActionResult<Item>> GetItem(int locationId, string date)
        {
            var location = await _context.Items.FindAsync(locationId, date);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        [HttpPut("{locationId}/{date}")]
        public async Task<IActionResult> PutItem(int locationId, string date, Item item)
        {
            item.LocationId = locationId;
            item.Date = date;

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{locationId}/{date}")]
        public async Task<ActionResult<Location>> PostItem(int locationId, string date, Item item)
        {
            item.LocationId = locationId;
            item.Date = date;

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { locationId = locationId, date = date }, item);
        }

        [HttpDelete("{locationId}/{date}")]
        public async Task<IActionResult> DeleteItem(int locationId, string date)
        {
            var item = await _context.Items.FindAsync(locationId, date);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ItemExists(int locationId, string date)
        {
            return _context.Items.Any(e => (e.LocationId == locationId && e.Date == date));
        }
    }
}
