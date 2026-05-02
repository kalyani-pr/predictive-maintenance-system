using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PredictiveMaintenance.API.Data;
using PredictiveMaintenance.API.Models;

namespace PredictiveMaintenance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SensorReadingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET sensor readings
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("{machineId}")]
        public IActionResult GetSensorReadingsByMachine(int machineId)
        {
            var readings = _context.SensorReadings
                .Where(sr => sr.MachineId == machineId)
                .OrderBy(sr => sr.Cycle)
                .ToList();

            if (!readings.Any())
                return NotFound("No sensor readings found for this machine.");

            return Ok(readings);
        }

        // POST sensor reading (NO AUTO ML)
        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public IActionResult CreateSensorReading([FromBody] SensorReading reading)
        {
            reading.RecordedAt = DateTime.Now;

            _context.SensorReadings.Add(reading);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Sensor data saved successfully. Click 'Run AI' to generate prediction.",
                reading
            });
        }
    }
}
