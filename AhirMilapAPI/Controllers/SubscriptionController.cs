using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using System.Text.Json;
using System.Text;
using AhirMilapAPI.Setup;
using log4net;
using Google.Api;

namespace AhirMilapAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _svc;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(SubscriptionController));
        public SubscriptionController(ISubscriptionService svc, ILocalizationService localizer)
        {
            _svc = svc;
            _localizer = localizer;
        }
        // ── GET /api/subscription/plans ─────────────────────────
        // No auth needed — Flutter calls this to show plan list
        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _svc.GetActivePlansAsync();
            return Ok(plans);
        }

        // ── POST /api/subscription/verify ───────────────────────
        // Flutter calls this right after Google billing succeeds.
        // Body: { purchaseToken, productId, orderId }
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.PurchaseToken))
                return BadRequest(new { success = false, error = "purchaseToken required" });

            var profileId = User.GetProfileId();
            if (profileId == null)
                return Unauthorized();

            var result = await _svc.VerifyAndActivateAsync(
                profileId, req.PurchaseToken, req.ProductId, req.OrderId);

            if (!result.Success)
                return BadRequest(new
                {
                    success = false,
                    error = result.Error
                });

            return Ok(new
            {
                success = true,
                plan = result.Plan,
                expiry = result.Expiry,
                message = "Subscription activated successfully"
            });
        }

        // ── GET /api/subscription/status ────────────────────────
        // App open check — returns active plan info or Free status
        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var profileId = User.GetProfileId();
            if (profileId == null) return Unauthorized();

            var status = await _svc.GetUserStatusAsync(profileId);
            return Ok(status);
        }

        // ── POST /api/subscription/webhook ──────────────────────
        // Google Pub/Sub calls this — NO JWT auth
        // MUST always return 200 or Google will retry endlessly
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook(
            [FromBody] GooglePubSubMessage message)
        {
            try
            {
                // Pub/Sub wraps payload in Base64 — decode it first
                var json = Encoding.UTF8.GetString(
                    Convert.FromBase64String(message.Message.Data));

                var notification = JsonSerializer.Deserialize<DeveloperNotification>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var n = notification?.SubscriptionNotification;
                if (n != null)
                {
                    switch (n.NotificationType)
                    {
                        case 2:  // RENEWED
                        case 4:  // PURCHASED
                            await _svc.HandleRenewalAsync(
                                n.PurchaseToken, n.SubscriptionId);
                            break;

                        case 3:  // CANCELED by user (still active until expiry)
                            await _svc.CancelSubscriptionAsync(n.PurchaseToken);
                            break;

                        case 13: // EXPIRED — no longer accessible
                            await _svc.ExpireSubscriptionAsync(n.PurchaseToken);
                            break;

                            // Other types (5=ON_HOLD, 7=RESTARTED, etc.) — log and ignore for now
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but ALWAYS return 200 — Google retries on non-200 indefinitely
                Console.WriteLine($"[Webhook Error] {ex.Message}");
            }

            return Ok();
        }
        
    }
}
