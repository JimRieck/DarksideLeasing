using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkside.Logging.Contracts.Requests
{
    public class GetLoggingRequest
    {
        public string LogLevel { get; set; }
        public string Application { get; set; }
        public Guid? TenantId { get; set; }
        public int TotalDays { get; set; } = 0;
    }
}
