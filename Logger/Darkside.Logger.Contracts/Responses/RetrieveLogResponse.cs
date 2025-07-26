
using Darkside.Logging.Datas.Models;

namespace Darkside.Logging.Logger.API.Responses
{
    public class RetrieveLogResponse
    {
        public int Count { get; set; } = 0;
        public List<Log> Logs { get; set; } = new List<Log>();
    }
}
