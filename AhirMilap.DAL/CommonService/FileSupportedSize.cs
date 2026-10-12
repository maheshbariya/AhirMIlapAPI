namespace AhirMilap.DAL.CommonService
{
    public class FileSupportedSize
    {
        //private readonly ISettingService _settService;

        //public FileSupportedSize(ISettingService settService)
        //{
        //    _settService = settService;
        //}

        public async Task<(bool, decimal?)> IsAllowPhotoSize(decimal fileSize)
        {
            //var getSetting = await _settService.GetSetting(); //get file size in megabyte 
            if (fileSize <= (5 * 1024 * 1024)) return (true, null); // set max size 5MB
            else return (false, Convert.ToDecimal(5));
        }

        //public async Task<(bool, decimal?)> IsAllowVideoSize(decimal fileSize)
        //{
        //    var getSetting = await _settService.GetSetting(); //get file size in megabyte 
        //    if (fileSize <= (getSetting.Data.MaxSizeOfVideo * 1024 * 1024)) return (true, null);
        //    else return (false, Convert.ToDecimal(getSetting.Data.MaxSizeOfVideo));
        //}

        //public async Task<(bool, decimal?)> IsAllowDocumentSize(decimal fileSize)
        //{
        //    var getSetting = await _settService.GetSetting(); //get file size in megabyte
        //    if (fileSize <= (getSetting.Data.MaxSizeOfDocument * 1024 * 1024)) return (true, null);
        //    else return (false, Convert.ToDecimal(getSetting.Data.MaxSizeOfDocument));
        //}
    }
}
