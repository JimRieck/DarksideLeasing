using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkside.Logging.Contracts.Requests
{
    public class AddLoggingRequest
    {
        public string LogLevel { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public Guid? TenantId { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
}
}
