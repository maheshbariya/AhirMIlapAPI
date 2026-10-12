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
    public class ProfileBlockController : ControllerBase
    {
        private readonly IProfileBlockService _service;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(ProfileBlockController));

        public ProfileBlockController(IProfileBlockService service, ILocalizationService localizer)
        {
            _service = service;
            _localizer = localizer;
        }
        [HttpPost("saveBlockUser")]
        public async Task<IActionResult> SaveBlockUser(ProfileBlockReqDTO model)
        {
            try
            {
                ProfileBlock profileblockUser = new ProfileBlock()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    CreatedBy = User.GetUserId(),
                };

                var res = await _service.AddBlockUser(profileblockUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "unblock")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("UserUnblockedSuccessfully")));
                    }

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserBlockedSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getBlockUsers/{profileFromId}")]
        public async Task<IActionResult> GetBlockUsers(Guid? profileFromId)
        {
            try
            {
                var res = await _service.GetBlockUsers(profileFromId, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NoBlockedUsersFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("unBlockUserById/{id}")]
        public async Task<IActionResult> DeleteBlockUserById(Guid? id)
        {
            try
            {
                var res = await _service.DeleteBlockUserById(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserUnblockedSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("BlockedUserNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToUnblockUser")));
            }
        }
    }
}
