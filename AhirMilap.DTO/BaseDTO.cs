using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.DTO
{
    public class BaseDTO
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
