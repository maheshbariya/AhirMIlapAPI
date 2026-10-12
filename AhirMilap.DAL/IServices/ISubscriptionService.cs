using AhirMilap.DTO;
using AhirMilap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.DAL.IServices
{
    public interface ISubscriptionService
    {
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync();

        Task<VerifyResult> VerifyAndActivateAsync(
            Guid? profileId,
            string purchaseToken,
            string productId,
            string orderId);

        Task<SubscriptionStatusDto> GetUserStatusAsync(Guid? profileId);

        // Called by webhook — type 2 (renewed) or 4 (purchased)
        Task HandleRenewalAsync(string purchaseToken, string sku);

        // Called by webhook — type 3 (user cancelled, still active until expiry)
        Task CancelSubscriptionAsync(string purchaseToken);

        // Called by webhook — type 13 (fully expired)
        Task ExpireSubscriptionAsync(string purchaseToken);
    }
}
