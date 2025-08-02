using MediatR;

namespace Darkside.Logging.Application.Features.Logging.Commands;



public class CreateLogCommand : IRequest<Guid>
{
    public string LogLevel { get; set; }
    public string Application { get; set; }
    public string Module { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public string Properties { get; set; }
}
