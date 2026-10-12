using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.Entity
{
    public class GoogleWebhookLog
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Payload { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? EventType { get; set; }

        public bool Processed { get; set; } = false;

        public string? Error { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    }
}
