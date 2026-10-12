using AhirMilap.DAL.IRepository;
using AhirMilap.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Repositories/SubscriptionRepository.cs
using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using AhirMilap.DAL.CommonService;

namespace AhirMilap.DAL.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;

        public SubscriptionRepository(IConfiguration configuration, GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }

        // Calls: usp_GetActivePlans
        public async Task<IEnumerable<SubscriptionPlan>> GetActivePlansAsync()
        {
            using (var connection = new SqlConnection(connString))
            {
                return await connection.QueryAsync<SubscriptionPlan>(
                "usp_GetActivePlans",
                commandType: CommandType.StoredProcedure);
            }
        }

        // Calls: usp_GetActiveSubscriptionByUser
        public async Task<UserSubscription?> GetActiveByUserIdAsync(Guid? profileId)
        {
            using (var connection = new SqlConnection(connString))
            {
                return await connection.QueryFirstOrDefaultAsync<UserSubscription>(
                "usp_GetActiveSubscriptionByUser",
                new { ProfileId = profileId },
                commandType: CommandType.StoredProcedure);
            }
        }

        // Calls: usp_GetSubscriptionByToken
        public async Task<UserSubscription?> GetByTokenAsync(string purchaseToken)
        {
            using (var connection = new SqlConnection(connString))
            {
                return await connection.QueryFirstOrDefaultAsync<UserSubscription>(
                "usp_GetSubscriptionByToken",
                new { PurchaseToken = purchaseToken },
                commandType: CommandType.StoredProcedure);
            }
        }

        // Calls: usp_UpsertUserSubscription (insert or update)
        public async Task UpsertSubscriptionAsync(UserSubscription sub)
        {
            using (var connection = new SqlConnection(connString))
            {
                await connection.ExecuteAsync(
                "usp_UpsertUserSubscription",
                new
                {
                    sub.ProfileId,
                    sub.PlanId,
                    sub.GoogleOrderId,
                    sub.PurchaseToken,
                    sub.Status,
                    sub.StartDate,
                    sub.ExpiryDate,
                    sub.AutoRenew
                },
                commandType: CommandType.StoredProcedure);
            }
        }

        // Calls: usp_UpdateSubscriptionStatus (used by webhook)
        public async Task UpdateStatusAsync(
            string purchaseToken, string status, DateTime expiryDate)
        {
            using (var connection = new SqlConnection(connString))
            {
                await connection.ExecuteAsync(
                "usp_UpdateSubscriptionStatus",
                new { PurchaseToken = purchaseToken, Status = status, ExpiryDate = expiryDate },
                commandType: CommandType.StoredProcedure);
            }
        }
    }
}
