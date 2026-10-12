using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using AhirMilapAPI.Setup;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhirMilapAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class FirebaseController : ControllerBase
    {
        private readonly IFirebaseService _service;
        private readonly ILocalizationService _localizer;
        ILog _log = LogManager.GetLogger(typeof(FirebaseController));
        private readonly FirebaseNotificationService _firebaseService;

        public FirebaseController(IFirebaseService service, FirebaseNotificationService firebaseService, ILocalizationService localizer)
        {
            _service = service;
            _firebaseService = firebaseService;
            _localizer = localizer;
        }

        [HttpPost("saveFirebaseToken")]
        public async Task<IActionResult> SaveFirebaseToken(FirebaseTokenReqDTO reqDTO)
        {
            try
            {
                string userId = User.GetUserId();
                FirebaseToken model = new FirebaseToken()
                {
                    UserId = userId,
                    FcmToken = reqDTO.FcmToken,
                    IsLogOut = reqDTO.IsLogOut,
                    CreatedBy = userId,
                };
                var res = await _service.AddFirebaseToken(model);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "deleted")
                        return Ok(new CommonResponse(Status.Success, _localizer.Get("TokenRemoved")));
                    else if(res.Data.CreatedBy == "exists")
                        return Ok(new CommonResponse(Status.Success, _localizer.Get("TokenAlreadyExists")));

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Data));
                }
                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NotFound"))); 
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("getFirebaseToken/{profileId}")]
        public async Task<IActionResult> GetFirebaseToken(Guid? profileId)
        {
            try
            { 
                var res = await _service.GetFirebaseToken(profileId, null);
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpPost("sendNotification")]
        public async Task<IActionResult> SendNotification(NotificationReqDTO reqDTO)
        {
            try
            {
                string userId = User.GetUserId();
                Notifications model = new Notifications()
                {
                    SenderProfileId = reqDTO.SenderProfileId,
                    ReceiverProfileId = reqDTO.ReceiverProfileId, 
                    Title = reqDTO.Title,
                    Message = reqDTO.Message,
                    NotificationType = reqDTO.NotificationType,
                    CreatedBy = userId,
                };
                var res = await _service.SaveNotifications(model);
                if (res.Data != null)
                {
                    var result = await _firebaseService.SendNotificationAsync(model);

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Data));
                }
                return Ok(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("getNotifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationReqDTO reqDto)
        {
            var messages = await _service.GetNotifications(reqDto, User.GetProfileId());
            if (messages.Data != null)
                return Ok(new CommonResponse(Status.Success, _localizer.Get("DataGatedSuccessfully"), messages.Data));

            return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
        }

        [HttpPut("readNotification/{notificationId}")]
        public async Task<IActionResult> ReadNotification(Guid? notificationId)
        {
            try
            {
                string userId = User.GetUserId();
                Notifications model = new Notifications()
                {
                    Id = notificationId,
                    CreatedBy = userId,
                };
                var res = await _service.SaveNotifications(model);
                if (res.Data != null)
                { 
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Data));
                }
                return Ok(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return StatusCode(500, new CommonResponse(Status.Error, ex.Message));
            }
        }
    }
}
