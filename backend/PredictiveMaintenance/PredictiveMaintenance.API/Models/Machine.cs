using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PredictiveMaintenance.API.Models
{
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string MachineType { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}