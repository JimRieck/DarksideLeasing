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

            var log = new Log
            {
                LogLevel = request.LogLevel,
                Application = request.Application,
                Module = request.Module,
                Message = request.Message,
                Exception = request.Exception,
                Properties = request.Properties,
                CreatedBy = "System", 
                CreatedDate = DateTime.Now
            };

            _context.Logs.Add(log);
    
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
