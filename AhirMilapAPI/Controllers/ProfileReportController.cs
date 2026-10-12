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
    public class ProfileReportController : ControllerBase
    {
        private readonly IProfileReportService _service;
        private readonly ILocalizationService _localizer;
        ILog _logger = LogManager.GetLogger(typeof(ProfileReportController));

        public ProfileReportController(IProfileReportService service, ILocalizationService localizer)
        {
            _service = service;
            _localizer = localizer;
        }
        [HttpPost("saveReportUser")]
        public async Task<IActionResult> SaveReportUser(ProfileReportReqDTO model)
        {
            try
            {
                ProfileReport profileReportUser = new ProfileReport()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    Reason = model.Reason,
                    Comments = model.Comments,
                    CreatedBy = User.GetUserId(),
                };

                var res = await _service.AddReportUser(profileReportUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "clearReport")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("UserReportClearedSuccessfully")));
                    }

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserReportedSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToProcessRequest")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getReportedUsers/{profileFromId}")]
        public async Task<IActionResult> GetReportUsers(Guid? profileFromId)
        {
            try
            {
                var res = await _service.GetReportUsers(profileFromId, User.GetUserId());
                if (res.Res != null)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("Success"), res.Res));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NoReportedUsersFound")));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }

        [HttpGet("ClearReportUserById/{id}")]
        public async Task<IActionResult> DeleteReportUserById(Guid? id)
        {
            try
            {
                var res = await _service.DeleteReportUserById(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("UserReportClearedSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("ReportedUserNotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("UnableToUnreportUser")));
            }
        }
    }
}
