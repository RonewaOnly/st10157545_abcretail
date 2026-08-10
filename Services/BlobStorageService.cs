using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace st10157545_abcretail.Services
{
    /// <summary>
    /// Concrete implementation of IBlobStorageService backed by Azure Blob
    /// Storage. Used to host product images and multimedia content, keeping
    /// large binary files out of Table Storage entirely (as recommended by
    /// Azure Storage best practices).
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
            var containerName = configuration["AzureStorage:BlobContainerName"] ?? "product-images";
            var serviceClient = new BlobServiceClient(connectionString);
            _containerClient = serviceClient.GetBlobContainerClient(containerName);
        }
        public async Task InitializeAsync()
        {
            // Public access lets product images be displayed directly in the
            // browser via their blob URL without extra SAS token plumbing.
            // For a production system holding anything sensitive, this should
            // be Private + short-lived SAS tokens instead.
            await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            // Prefix with a GUID to avoid filename collisions between
            // different uploads that happen to share the same file name.
            var blobName = $"{Guid.NewGuid()}-{fileName}";
            var blobClient = _containerClient.GetBlobClient(blobName);
            var headers = new BlobHttpHeaders { ContentType = contentType };
            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers });
            return blobClient.Uri.ToString();
        }
        public async Task DeleteFileAsync(string blobNameOrUrl)
        {
            // Accept either a bare blob name or a full URL and extract the
            // blob name from either form.
            var blobName = blobNameOrUrl.Contains('/')
                ? blobNameOrUrl.Split('/').Last()
                : blobNameOrUrl;
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
    }
}
