using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PredictiveMaintenance.API.Data;
using PredictiveMaintenance.API.ML;
using PredictiveMaintenance.API.Models;

namespace PredictiveMaintenance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PredictionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly MLInferenceService _mlService;

        public PredictionsController(
            ApplicationDbContext context,
            MLInferenceService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        // GET predictions for a machine
        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("{machineId}")]
        public IActionResult GetPredictionsByMachine(int machineId)
        {
            var predictions = _context.Predictions
                .Where(p => p.MachineId == machineId)
                .OrderByDescending(p => p.PredictionDate)
                .ToList();

            if (!predictions.Any())
                return NotFound("No predictions found for this machine.");

            return Ok(predictions);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("trend/{machineId}")]
        public IActionResult GetDegradationTrend(int machineId)
        {
            var trend = _context.Predictions
                .Where(p => p.MachineId == machineId)
                .OrderBy(p => p.PredictionDate)
                .Select(p => new
                {
                    predictionDate = p.PredictionDate,
                    healthIndex = p.HealthIndex
                })
                .ToList();

            if (trend.Count == 0)
                return NotFound("No prediction history found for this machine.");

            return Ok(trend);
        }

        // MAIN ML PREDICTION ENDPOINT
        [Authorize(Roles = "Admin")]
        [HttpPost("run/{machineId}")]
        public IActionResult RunPrediction(int machineId)
        {
            // 1️ Get latest sensor reading
            var latestReading = _context.SensorReadings 
                .Where(s => s.MachineId == machineId)
                .OrderByDescending(s => s.Cycle)
                .FirstOrDefault();

            if (latestReading == null)
                return NotFound("No sensor data found for this machine.");

            // 2️ Run ML model (Cycle, Pressure, Vibration, Temperature)
            var result = _mlService.Predict(
                latestReading.Cycle,
                latestReading.Pressure,
                latestReading.Vibration,
                latestReading.Temperature
            );

            // 3️ Save prediction
            var prediction = new Prediction
            {
                MachineId = machineId,
                PredictedRUL = result.RUL,
                HealthIndex = result.HealthIndex,
                FailureProbability = result.FailureProbability,
                RiskLevel = result.RiskLevel,
                PredictionDate = DateTime.Now
            };

            _context.Predictions.Add(prediction);

            // 4 Auto-create Alert if critical (Prevent duplicates)
            var existingActiveAlert = _context.Alerts
                .FirstOrDefault(a =>
                    a.MachineId == machineId &&
                    !a.IsAcknowledged &&
                    a.RiskLevel == "Critical");

            if (result.RiskLevel == "Critical" && existingActiveAlert == null)
            {
                var alert = new Alert
                {
                    MachineId = machineId,
                    RiskLevel = "Critical",
                    Description = "AI detected critical failure risk",
                    IsAcknowledged = false,
                    CreatedAt = DateTime.Now
                };

                _context.Alerts.Add(alert);
            }

            _context.SaveChanges();

            return Ok(prediction);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("latest/{machineId}")]
        public IActionResult GetLatestPrediction(int machineId)
        {
            var latest = _context.Predictions
                .Where(p => p.MachineId == machineId)
                .OrderByDescending(p => p.PredictionDate)
                .FirstOrDefault();

            if (latest == null)
                return NotFound("No prediction found for this machine.");

            return Ok(latest);
        }
    }
}
