using System.ComponentModel.DataAnnotations;

namespace PredictiveMaintenance.API.Models
{
    public class SensorReading
    {
        [Key]
        public long ReadingId { get; set; }
        public int MachineId { get; set; }
        public int Cycle { get; set; }
        public double? Temperature { get; set; }
        public double? Pressure { get; set; }
        public double? Vibration { get; set; }
        public double? Speed { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}