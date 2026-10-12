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
    public class ProfileLikeController : ControllerBase
    {
        private readonly IProfileLikeService _service;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(ProfileLikeController));
        private readonly FirebaseNotificationService _firebaseService;
        private readonly IFirebaseService _fService;

        public ProfileLikeController(IProfileLikeService service, ILocalizationService localizer, FirebaseNotificationService firebaseNotService, IFirebaseService fService)
        {
            _service = service;
            _localizer = localizer;
            _firebaseService = firebaseNotService;
            _fService = fService;
        }
        [HttpPost("saveLikeUser")]
        public async Task<IActionResult> SaveLikeUser(ProfileLikeReqDTO model)
        {
            try
            {
                string userId = User.GetUserId();
                ProfileLike profileLikeUser = new ProfileLike()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    CreatedBy = userId,
                };

                var res = await _service.AddLikeUser(profileLikeUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "Dislike")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("UserDislikedSuccessfully")));
                    }

                    //Send push notification
                    Notifications notification = new Notifications()
                    {
                        SenderProfileId = model.ProfileIdFrom,
                        ReceiverProfileId = model.ProfileIdTo,  
                        NotificationType = "PROFILE_LIKE",
                        CreatedBy = userId,
                    }; 
                    await _firebaseService.SendNotificationAsync(notification);

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserLikedSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getLikeUsers/{profileFromId}")]
        public async Task<IActionResult> GetLikeUsers(Guid? profileFromId)
        {
            try
            {
                var res = await _service.GetLikeUsers(profileFromId, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NoLikedUsersFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("DislikeUserById/{id}")]
        public async Task<IActionResult> DeleteLikeUserById(Guid? id)
        {
            try
            {
                var res = await _service.DeleteLikeUserById(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserDislikedSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("LikedUserNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToDislikeUser")));
            }
        }
    }
}
