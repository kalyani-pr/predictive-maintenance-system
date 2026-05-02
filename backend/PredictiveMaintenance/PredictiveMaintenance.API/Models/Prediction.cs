using System.ComponentModel.DataAnnotations;

namespace PredictiveMaintenance.API.Models
{
    public class Prediction
    {
        [Key]
        public long PredictionId { get; set; }
        public int MachineId { get; set; }
        public double PredictedRUL { get; set; }
        public double HealthIndex { get; set; }
        public double FailureProbability { get; set; }
        public string RiskLevel { get; set; }
        public DateTime PredictionDate { get; set; }
    }
}
