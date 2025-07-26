using Darkside.Logging.Datas.Models;
using MediatR;
using System;
using System.Collections.Generic;

public class GetLogsQuery : IRequest<List<Log>>
{
    public string LogLevel { get; set; }
    public string Application { get; set; }
    public Guid? TenantId { get; set; }
    public int TotalDays { get; set; } = 0;
}