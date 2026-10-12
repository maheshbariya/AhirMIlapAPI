using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.Entity
{
   
    public class UserSubscription
    {
        public long Id { get; set; }
        public Guid? ProfileId { get; set; }
        public int PlanId { get; set; }
        public string GoogleOrderId { get; set; } = string.Empty;
        public string PurchaseToken { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool AutoRenew { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        // Populated by JOIN in usp_GetActiveSubscriptionByUser
        public string? PlanName { get; set; }
    }
}
