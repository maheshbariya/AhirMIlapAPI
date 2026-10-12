using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilapAPI.Setup;
using AhirMilap.DAL.Services;
using AhirMilap.Entity;
using AhirMilap.DAL.CommonService;

namespace AhirMilapAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        private readonly ILocalizationService _localizer;
        private readonly FirebaseNotificationService _firebaseService;
        ILog _logger = LogManager.GetLogger(typeof(DashboardController));


        public DashboardController(IDashboardService service, ILocalizationService localizer, FirebaseNotificationService firebaseService)
        {
            _service = service;
            _localizer = localizer;
            _firebaseService = firebaseService;
        }

        [Authorize]
        [HttpGet("getProfilesList")]
        public async Task<IActionResult> GetProfilesListOld()
        {
            try
            {
                Filter filters = new Filter();
                var res = await _service.GetProfilesList(filters, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }
        [Authorize]
        [HttpPost("getProfilesList")]
        public async Task<IActionResult> GetProfilesList(Filter? filters)
        {
            try
            {
                var res = await _service.GetProfilesList(filters, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }
        [Authorize]
        [HttpGet("getProfileDetailsById/{profileId}")]
        public async Task<IActionResult> GetProfileDetailsById(Guid? profileId)
        {
            try
            {
                var res = await _service.GetProfileDetailsById(User.GetProfileId(),profileId);
                if (res.Data != null)
                {
                    //Send push notification
                    if(res.Data.IsSentNotification)
                    {
                        Notifications notification = new Notifications()
                        {
                            SenderProfileId = User.GetProfileId(),
                            ReceiverProfileId = profileId,
                            NotificationType = "PROFILE_VIEW",
                            CreatedBy = User.GetUserId(),
                        };
                        await _firebaseService.SendNotificationAsync(notification);
                    } 
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Data));
                } 
                return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }


    }
}