using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkside.Logging.Application.Features.Upload.Commands
{
    public class UploadFileCommand : IRequest<Guid>
    {
        public IFormFile File { get; set; }
        public string ContainerName { get; set; }
    }
}
