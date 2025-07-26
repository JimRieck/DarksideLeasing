using Darkside.Logging.Application.Features.Logging.Commands;
using Darkside.Logging.Datas.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class CreateLogCommandHandler(GPSDocumentGenieDataContext context) : IRequestHandler<CreateLogCommand, Guid>
{
    private readonly GPSDocumentGenieDataContext _context = context;

    public async Task<Guid> Handle(CreateLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Set up the database context

            var tenant = _context.Tenants.FirstOrDefault();

            var log = new Log
            {
                LogLevel = request.LogLevel,
                Application = request.Application,
                Module = request.Module,
                TenantId = tenant.TenantId,
                Message = request.Message,
                Exception = request.Exception,
                Properties = request.Properties,
                CreatedBy = "System", 
                CreatedDate = DateTime.UtcNow
            };

            _context.Logs.Add(log);
            var connStr = _context.Database.GetConnectionString();
            Console.WriteLine($"Connection string is {connStr}");
            await _context.SaveChangesAsync(cancellationToken);
            return log.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while handling the CreateLogCommand: {ex.Message}, {ex.InnerException?.Message}");
        }

        return Guid.Empty;
    }
}
