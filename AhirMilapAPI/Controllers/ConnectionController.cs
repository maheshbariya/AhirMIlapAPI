using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IServices;
using AhirMilap.DAL.Services;
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
    public class ConnectionController : ControllerBase
    {
        private readonly IConnectionService _service;
        private readonly ILocalizationService _localizer;
        private readonly FirebaseNotificationService _firebaseService;
        ILog _logger = LogManager.GetLogger(typeof(ConnectionController));
        public ConnectionController(IConnectionService service, ILocalizationService localizer, FirebaseNotificationService firebaseService)
        {
            _service = service;
            _localizer = localizer;
            _firebaseService = firebaseService;
        }
        [HttpPost("SendRequest")]
        public async Task<IActionResult> RequestConnection(ConnectionReqDTO model)
        {
            try
            {
                string userId = User.GetUserId();
                Connection ConnectionUser = new Connection()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    CreatedBy = userId,
                    UpdatedBy = userId,
                };

                var res = await _service.RequestConnection(ConnectionUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "Delete")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("RequestDeletedSuccessfully")));
                    }

                    //Send push notification
                    Notifications notification = new Notifications()
                    {
                        SenderProfileId = model.ProfileIdFrom,
                        ReceiverProfileId = model.ProfileIdTo,
                        NotificationType = "REQUEST_SENT",
                        CreatedBy = userId,
                    };
                    await _firebaseService.SendNotificationAsync(notification);

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("SendRequestSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getConnectionUsers/{actionId}")]
        public async Task<IActionResult> GetConnectionUsers(int actionId)
        {
            try
            {
                var res = await _service.GetConnectionUsers(User.GetProfileId(), actionId);
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, actionId == 1 ? _localizer.Get("NoReceivedRequestAvailable") : actionId == 2 ? _localizer.Get("NoAcceptedRequestAvailable") : actionId == 3 ? _localizer.Get("NoSentRequestAvailable") : _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("ConnectionAction/{id}")]
        public async Task<IActionResult> ConnectionActionById(Guid? id, int? ActionId, Guid? profileId)
        {
            try
            {
                var res = await _service.ConnectionActionById(id, User.GetUserId(), ActionId);
                if (res.Data)
                {
                    //Send push notification
                    if (profileId != null && ActionId == 1)
                    {
                        Notifications notification = new Notifications()
                        {
                            SenderProfileId = User.GetProfileId(),
                            ReceiverProfileId = profileId,
                            NotificationType = "REQUEST_ACCEPTED",
                            CreatedBy = User.GetUserId(),
                        };
                        await _firebaseService.SendNotificationAsync(notification);
                    }
                    return Ok(new CommonResponse(Status.Success, ActionId == 1 ? _localizer.Get("RequestApprovedSuccessfully") : _localizer.Get("RequestDeletedSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
        }
    }
}
