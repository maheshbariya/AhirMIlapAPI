using AhirMilap.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.DAL.IRepository
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync();
        Task<UserSubscription?> GetActiveByUserIdAsync(Guid? profileId);
        Task<UserSubscription?> GetByTokenAsync(string purchaseToken);
        Task UpsertSubscriptionAsync(UserSubscription sub);
        Task UpdateStatusAsync(string purchaseToken,
                                                string status, DateTime expiryDate);
    }
}
