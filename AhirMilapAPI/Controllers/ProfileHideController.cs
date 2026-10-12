using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using AhirMilapAPI.Setup;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AhirMilapAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProfileHideController : ControllerBase
    {
        private readonly IProfileHideService _service;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(ProfileHideController));

        public ProfileHideController(IProfileHideService service, ILocalizationService localizer)
        {
            _service = service;
            _localizer = localizer;
        }
        [HttpPost("saveHideUser")]
        public async Task<IActionResult> saveHideUser(ProfileHideReqDTO model)
        {
            try
            {
                ProfileHide profilehideUser = new ProfileHide()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    CreatedBy = User.GetUserId(),
                };

                var res = await _service.AddHideUser(profilehideUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "unhide")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("UserUnhiddenSuccessfully")));
                    }

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserHiddenSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getHideUsers/{profileFromId}")]
        public async Task<IActionResult> GetHideUsers(Guid? profileFromId)
        {
            try
            {
                var res = await _service.GetHideUsers(profileFromId, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NoHiddenUsersFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("unHideUserById/{id}")]
        public async Task<IActionResult> DeleteHideUserById(Guid? id)
        {
            try
            {
                var res = await _service.DeleteHideUserById(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserUnhiddenSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("HiddenUserNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToUnhideUser")));
            }
        }
    }
}
