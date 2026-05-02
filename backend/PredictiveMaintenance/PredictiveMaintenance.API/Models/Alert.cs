using System.ComponentModel.DataAnnotations;

namespace PredictiveMaintenance.API.Models
{
    public class Alert
    {
        [Key]
        public int AlertId { get; set; }
        public int MachineId { get; set; }
        public string RiskLevel { get; set; }
        public string Description { get; set; }
        public bool IsAcknowledged { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
    }
}