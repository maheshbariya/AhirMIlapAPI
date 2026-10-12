namespace AhirMilap.DAL.CommonService
{
    public class FileSupportedType
    {
        string[] photoTypes = { ".png", ".jpg", ".gif", ".jpeg" };
        string[] videoTypes = { ".mp4", ".avi", ".mpeg", ".dat", ".3gp", ".mov" };
        string[] docTypes = { ".png", ".jpg", ".gif", ".jpeg", ".mp4", ".avi", ".mpeg", ".dat", ".3gp", ".pdf", ".docx", ".xls" };
 
        public bool IsAllowPhotoFile(string fileExt)
        { 
            return photoTypes.Contains(fileExt.ToLower());
        }

        public bool IsAllowVideoFile(string fileExt)
        { 
            return videoTypes.Contains(fileExt.ToLower());
        }

        public bool IsAllowDocumentFile(string fileExt)
        { 
            return docTypes.Contains(fileExt.ToLower());
        } 

        public int ImageOrVideoType(string fileExt)
        {
            int type = 0;
            if (IsAllowPhotoFile(fileExt)) type = 1;
            else if (IsAllowVideoFile(fileExt)) type = 2; 
            return type;
        } 
    }
}
