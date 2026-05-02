using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PredictiveMaintenance.API.Data;
using PredictiveMaintenance.API.Models;
using System.Linq;

namespace PredictiveMaintenance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MachineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MachineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Machine
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet]
        public IActionResult GetAllMachines()
        {
            var machines = _context.Machines.ToList();
            return Ok(machines);
        }

        // GET: api/Machine/5
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("{id}")]
        public IActionResult GetMachine(int id)
        {
            var machine = _context.Machines.Find(id);

            if (machine == null)
                return NotFound();

            return Ok(machine);
        }

        // POST: api/Machine
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddMachine([FromBody] Machine machine)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            machine.CreatedAt = DateTime.Now;

            _context.Machines.Add(machine);
            _context.SaveChanges();

            return Ok(machine);
        }

        // PUT: api/Machine/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateMachine(int id, [FromBody] Machine updatedMachine)
        {
            var machine = _context.Machines.Find(id);

            if (machine == null)
                return NotFound();

            machine.MachineName = updatedMachine.MachineName;
            machine.MachineType = updatedMachine.MachineType;
            machine.Location = updatedMachine.Location;
            machine.IsActive = updatedMachine.IsActive;

            _context.SaveChanges();

            return Ok(machine);
        }

        // DELETE: api/Machine/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteMachine(int id)
        {
            var machine = _context.Machines.Find(id);

            if (machine == null)
                return NotFound();

            _context.Machines.Remove(machine);
            _context.SaveChanges();

            return Ok("Machine deleted successfully.");
        }

        // GET: api/status/5
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("status/{id}")]
        public IActionResult GetMachineStatus(int id)
        {
            var machine = _context.Machines.Find(id);

            if (machine == null)
                return NotFound();

            var activeAlerts = _context.Alerts
                .Where(a => a.MachineId == id && !a.IsAcknowledged)
                .ToList();

            string status = "Healthy";

            if (activeAlerts.Any(a => a.RiskLevel == "Critical"))
                status = "Critical";
            else if (activeAlerts.Any(a => a.RiskLevel == "Warning"))
                status = "Warning";

            return Ok(new
            {
                MachineId = id,
                MachineName = machine.MachineName,
                Status = status
            });

        }

        // Dashboard
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("system-summary")]
        public IActionResult GetSystemSummary()
        {
            var machines = _context.Machines.ToList();

            int totalMachines = machines.Count;

            int healthy = 0;
            int warning = 0;
            int critical = 0;

            foreach (var machine in machines)
            {
                var latestPrediction = _context.Predictions
                    .Where(p => p.MachineId == machine.MachineId)
                    .OrderByDescending(p => p.PredictionDate)
                    .FirstOrDefault();

                if (latestPrediction == null)
                {
                    //healthy++;
                    continue;
                }

                if (latestPrediction.RiskLevel == "Critical")
                    critical++;
                else if (latestPrediction.RiskLevel == "Warning")
                    warning++;
                else
                    healthy++;
            }

            int activeAlertsCount = _context.Alerts
                .Count(a => !a.IsAcknowledged);

            return Ok(new
            {
                TotalMachines = totalMachines,
                Healthy = healthy,
                Warning = warning,
                Critical = critical,
                ActiveAlerts = activeAlertsCount
            });
        }
    }
}
