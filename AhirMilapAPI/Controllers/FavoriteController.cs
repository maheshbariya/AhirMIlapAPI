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
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _service;
        private readonly ILocalizationService _localizer;
        ILog _log = LogManager.GetLogger(typeof(FavoriteController));

        public FavoriteController(IFavoriteService service, ILocalizationService localizer)
        {
            _service = service;
            _localizer = localizer;
        }

        [HttpPost("saveFavoriteUser")]
        public async Task<IActionResult> SaveFavoriteUser(FavoriteReqDTO model)
        {
            try
            { 
                FavoriteUser favoriteUser = new FavoriteUser()
                {
                    ProfileIdFrom = model.ProfileIdFrom,
                    ProfileIdTo = model.ProfileIdTo,
                    CreatedBy = User.GetUserId(),
                };

                var res = await _service.AddFavoriteUser(favoriteUser);
                if (res.Data != null)
                {
                    if (res.Data.CreatedBy == "deleted")
                    {
                        return Ok(new CommonResponse(Status.Error, _localizer.Get("FavoriteUserDeletedSuccessfully")));
                    }

                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
                }

                return Ok(new CommonResponse(Status.Error, _localizer.Get("Error")));
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return Ok(new CommonResponse(Status.Error, ex.Message));
            }
        }


        [HttpGet("getFavoriteUsers/{profileFromId}")]
        public async Task<IActionResult> GetFavoriteUsers(Guid? profileFromId)
        {
            try
            {
                var res = await _service.GetFavoriteUsers(profileFromId, User.GetUserId());
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

        [HttpGet("deleteFavoriteUserById/{id}")]
        public async Task<IActionResult> DeleteFavoriteUserById(Guid? id)
        {
            try
            {
                var res = await _service.DeleteFavoriteUserById(id);
                if (res.Data)
                {
                    return Ok(new CommonResponse(Status.Success, _localizer.Get("DataDeletedSuccessfully")));
                }

                return Ok(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
            }
            catch (Exception ex)
            {
                return Ok(new CommonResponse(Status.Error, _localizer.Get("DeleteRecordAssociated")));
            }
        }
    }
}
