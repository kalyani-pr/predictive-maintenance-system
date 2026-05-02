using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PredictiveMaintenance.API.Data;
using PredictiveMaintenance.API.Models;

namespace PredictiveMaintenance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View alerts
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet]
        public IActionResult GetAllAlerts()
        {
            var alerts = _context.Alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return Ok(alerts);
        }

        // Create alerts
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateAlert([FromBody] Alert alert)
        {
            alert.CreatedAt = DateTime.Now;
            alert.IsAcknowledged = false;

            _context.Alerts.Add(alert);
            _context.SaveChanges();

            return Ok(alert);
        }

        // PUT: api/Alerts/5 (Acknowledge alerts)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/acknowledge")]
        public IActionResult AcknowledgeAlert(int id)
        {
            var alert = _context.Alerts.Find(id);

            if (alert == null)
                return NotFound();

            alert.IsAcknowledged = true;
            alert.AcknowledgedAt = DateTime.Now;

            _context.SaveChanges();

            return Ok(alert);
        }
    }
}
