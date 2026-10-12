using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.Entity
{
    public class DashboardProfilesBasicDetails
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string HighestEducation { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string ProfileFilePath { get; set; } = string.Empty;
        public string ProfileStatus { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } = false;
        public bool IsLiked { get; set; } = false;
        public int TotalRecords { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
         
    }
}
