using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace AhirMilap.DAL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _repo;
        private readonly IGooglePlayService _google;

        public SubscriptionService(
            ISubscriptionRepository repo,
            IGooglePlayService google)
        {
            _repo = repo;
            _google = google;
        }

        public Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
            => _repo.GetActivePlansAsync();

        public async Task<VerifyResult> VerifyAndActivateAsync(
            Guid? profileId, string purchaseToken,
            string productId, string orderId)
        {
            // Step 1: Verify the token is real by calling Google Play API
            var gpSub = await _google.GetSubscriptionAsync(productId, purchaseToken);
            if (gpSub == null)
                return VerifyResult.Fail("Purchase token not found in Google Play");

            long? expiryMs = gpSub.ExpiryTimeMillis;

            if (!expiryMs.HasValue)
                return VerifyResult.Fail("Invalid expiry time from Google");

            var expiry = DateTimeOffset
                .FromUnixTimeMilliseconds(expiryMs.Value)
                .UtcDateTime;

            if (expiry < DateTime.UtcNow)
                return VerifyResult.Fail("Subscription has already expired");

            // Step 3: Find matching plan by SKU in our DB
            var plans = await _repo.GetActivePlansAsync();
            var plan = plans.FirstOrDefault(p => p.GoogleSku == productId);
            if (plan == null)
                return VerifyResult.Fail($"Unknown product SKU: {productId}");

            // Step 4: Save to DB via SP (usp_UpsertUserSubscription)
            await _repo.UpsertSubscriptionAsync(new UserSubscription
            {
                ProfileId = profileId.Value,
                PlanId = plan.Id,
                GoogleOrderId = orderId,
                PurchaseToken = purchaseToken,
                Status = "Active",
                StartDate = DateTime.UtcNow,
                ExpiryDate = expiry,
                AutoRenew = gpSub.AutoRenewing ?? true,
            });

            return VerifyResult.Ok(plan.Name, expiry);
        }

        public async Task<SubscriptionStatusDto> GetUserStatusAsync(Guid? profileId)
        {
            var sub = await _repo.GetActiveByUserIdAsync(profileId);
            if (sub == null)
                return new SubscriptionStatusDto { IsActive = false, Status = "Free" };

            return new SubscriptionStatusDto
            {
                IsActive = true,
                Plan = sub.PlanName,
                ExpiryDate = sub.ExpiryDate,
                Status = "Active"
            };
        }

        // Webhook — type 2 (renewed) or 4 (new purchase)
        public async Task HandleRenewalAsync(string purchaseToken, string sku)
        {
            var gpSub = await _google.GetSubscriptionAsync(sku, purchaseToken);
            if (!gpSub.ExpiryTimeMillis.HasValue)
                return;

            var expiry = DateTimeOffset
                .FromUnixTimeMilliseconds(gpSub.ExpiryTimeMillis.Value)
                .UtcDateTime;

            await _repo.UpdateStatusAsync(purchaseToken, "Active", expiry);
        }

        // Webhook — type 3 (cancelled, still valid until ExpiryDate)
        public async Task CancelSubscriptionAsync(string purchaseToken)
        {
            var sub = await _repo.GetByTokenAsync(purchaseToken);
            if (sub == null) return;
            // Keep original ExpiryDate — user keeps access until then
            await _repo.UpdateStatusAsync(purchaseToken, "Cancelled", sub.ExpiryDate);
        }

        // Webhook — type 13 (fully expired)
        public async Task ExpireSubscriptionAsync(string purchaseToken)
            => await _repo.UpdateStatusAsync(purchaseToken, "Expired", DateTime.UtcNow);
    }
}
