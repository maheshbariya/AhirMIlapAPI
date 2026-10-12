using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AhirMilap.Entity;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.AndroidPublisher.v3;

namespace AhirMilap.DAL.Services
{
    public interface IGooglePlayService
    {
        // Returns null if token is invalid or not found
        Task<SubscriptionPurchase?> GetSubscriptionAsync(
            string productId, string purchaseToken);
    }

    public class GooglePlayService : IGooglePlayService
    {
        private readonly AndroidPublisherService _publisher;
        private readonly string _packageName;

        public GooglePlayService(IConfiguration cfg)
        {
            _packageName = cfg["Google:PackageName"]
                ?? throw new InvalidOperationException("Google:PackageName missing in appsettings");

            // Load the JSON key file downloaded from Google Cloud Console
            // → IAM → Service Accounts → Keys → Add Key → JSON
            var credential = GoogleCredential
                .FromFile(cfg["Google:ServiceAccountJsonPath"]
                    ?? throw new InvalidOperationException("Google:ServiceAccountJsonPath missing"))
                .CreateScoped(AndroidPublisherService.Scope.Androidpublisher);

            // AndroidPublisherService = the actual Google Play API client
            // It handles HTTP, auth headers, retries for you
            _publisher = new AndroidPublisherService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "MatroMoney"
                });
        }

        /// <summary>
        /// Calls Google Play API to verify a purchase token.
        ///
        /// Internally calls:
        /// GET androidpublisher.googleapis.com/androidpublisher/v3/
        ///     applications/{packageName}/purchases/subscriptions/
        ///     {productId}/tokens/{purchaseToken}
        ///
        /// Returns SubscriptionPurchase with:
        ///   - ExpiryTimeMillis  → when subscription expires (Unix ms)
        ///   - AutoRenewing      → true = will auto-renew
        ///   - PaymentState      → 0=pending, 1=received, 2=free trial
        ///   - OrderId           → GPA.xxx order reference
        /// Returns null if token not found (invalid/fake).
        /// </summary>
        public async Task<SubscriptionPurchase?> GetSubscriptionAsync(
            string productId, string purchaseToken)
        {
            try
            {
                return await _publisher.Purchases.Subscriptions
                    .Get(_packageName, productId, purchaseToken)
                    .ExecuteAsync();
            }
            catch (Google.GoogleApiException ex)
                when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                // Token not found — purchase was invalid or already consumed
                return null;
            }
            // Let other exceptions (network, 403 auth) propagate up
        }
    }
}
