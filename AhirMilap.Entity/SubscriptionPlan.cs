using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.Entity
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GoogleSku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "INR";
        public int DurationDays { get; set; }
        public string? Features { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
