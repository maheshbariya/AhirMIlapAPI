using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.DTO
{
    public class VerifyRequest
    {
        public string PurchaseToken { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty; // the SKU
        public string OrderId { get; set; } = string.Empty;
    }

    // Returned by GET /api/subscription/status
    public class SubscriptionStatusDto
    {
        public bool IsActive { get; set; }
        public string? Plan { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Status { get; set; } = "Free";
    }

    // Google Pub/Sub webhook body (outer envelope)
    public class GooglePubSubMessage
    {
        public PubSubData Message { get; set; } = new();
    }

    public class PubSubData
    {
        public string Data { get; set; } = string.Empty; // Base64
    }

    // Decoded from Pub/Sub Data field (inner JSON)
    public class DeveloperNotification
    {
        public string Version { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public long EventTimeMillis { get; set; }
        public SubscriptionNotificationData? SubscriptionNotification { get; set; }
    }

    public class SubscriptionNotificationData
    {
        public int NotificationType { get; set; }  // 2=renewed, 3=cancelled, 13=expired
        public string PurchaseToken { get; set; } = string.Empty;
        public string SubscriptionId { get; set; } = string.Empty; // the SKU
    }
}
