using Darkside.Logging.Application.Features.Upload.Commands;
using Darkside.Logging.Datas.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Darkside.Logging.Application.Features.Upload.Handlers
{
    public class UploadToBlobStorageCommandHandler : IRequestHandler<UploadFileCommand, Guid>
    {
        private readonly GPSDocumentGenieDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public UploadToBlobStorageCommandHandler(GPSDocumentGenieDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || string.IsNullOrEmpty(request.ContainerName))
                throw new ArgumentException("File and ContainerName are required");

            // Upload to Blob Storage
            var containerClient = _blobServiceClient.GetBlobContainerClient(request.ContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(request.File.FileName);
            using (var stream = request.File.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = request.File.ContentType }, cancellationToken: cancellationToken);
            }

            // Insert a record in the UploadFiles table
            var uploadFile = new UploadFile
            {
                UploadFileId = Guid.NewGuid(),
                Status = "Uploaded",
                TemplateId = Guid.Empty, // Set to a valid template if available
                UserId = Guid.Empty, // Set to a valid user if available
                GeneratedUrl = blobClient.Uri.ToString(),
                CreatedBy = "system", // Replace with actual user if available
                CreatedDate = DateTime.UtcNow,
                UpdatedBy = null,
                UpdatedDate = null,
                TenantId = Guid.Empty, // Set to a valid tenant if available
                ModifiedBy = null,
                ModifiedOn = null
            };
            _context.Add(uploadFile);
            await _context.SaveChangesAsync(cancellationToken);

            return uploadFile.UploadFileId;
        }
    }
}
