using Darkside.Logging.Datas.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GetLogsQueryHandler : IRequestHandler<GetLogsQuery, List<Log>>
{
    private readonly GPSDocumentGenieDataContext _context;

    public GetLogsQueryHandler(GPSDocumentGenieDataContext context)
    {
        _context = context;
    }

    public async Task<List<Log>> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Logs.AsQueryable();

        if (!string.IsNullOrEmpty(request.LogLevel))
            query = query.Where(l => l.LogLevel == request.LogLevel);

        if (!string.IsNullOrEmpty(request.Application))
            query = query.Where(l => l.Application == request.Application);

        if (request.TenantId.HasValue)
            query = query.Where(l => l.TenantId == request.TenantId);

        if (request.TotalDays > 0)
            query = query.Where(l => l.Timestamp >= DateTime.UtcNow.AddDays(-request.TotalDays));

        return await query
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
