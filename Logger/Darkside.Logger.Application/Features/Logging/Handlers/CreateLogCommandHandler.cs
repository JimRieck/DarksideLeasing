using Darkside.Logging.Datas.Models;
using Darkside.Logging.Application.Features.Logging.Commands;
using MediatR;

public class CreateLogCommandHandler(GPSDocumentGenieDataContext context) : IRequestHandler<CreateLogCommand, Guid>
{
    private readonly GPSDocumentGenieDataContext _context = context;

    public async Task<Guid> Handle(CreateLogCommand request, CancellationToken cancellationToken)
    {
        var log = new Log
        {
            LogLevel = request.LogLevel,
            Application = request.Application,
            Module = request.Module,
            TenantId = request.TenantId,
            Message = request.Message,
            Exception = request.Exception,
            Properties = request.Properties,
            CreatedBy = "System", 
            CreatedDate = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
        return log.Id;
    }
}
