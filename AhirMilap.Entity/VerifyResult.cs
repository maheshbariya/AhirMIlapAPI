using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhirMilap.Entity
{
    public class VerifyResult
    {
        public bool Success { get; private set; }
        public string? Error { get; private set; }
        public string? Plan { get; private set; }
        public DateTime? Expiry { get; private set; }

        // Static factory — keeps calling code clean and readable
        public static VerifyResult Ok(string plan, DateTime expiry)
            => new VerifyResult { Success = true, Plan = plan, Expiry = expiry };

        public static VerifyResult Fail(string error)
            => new VerifyResult { Success = false, Error = error };
    }
}
