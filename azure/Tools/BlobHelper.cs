using System.Text;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;

namespace Visma.Business.WebhookExample.Tools {
    public class BlobHelper {
        public const string InvalidSignature = "InvalidSignature";
        readonly BlobContainerClient blobContainerClient;
        public BlobHelper(BlobContainerClient containerClient) {
            this.blobContainerClient = containerClient;
        }
        public async Task WriteAlert(string type, string message) {
            var name = DateTime.Now.Ticks.ToString() + type;
            var blobClient = blobContainerClient.GetBlobClient(name);
            using var writeStream = await blobClient.OpenWriteAsync(true);
            var streamWriter = new StreamWriter(writeStream);
            await streamWriter.WriteAsync(message);
            await streamWriter.FlushAsync();
        }

        public async Task<string?> ReadBlob(string name) {
            var blobClient = blobContainerClient.GetBlobClient(name);
            if (await blobClient.ExistsAsync())
            {
                using var memoryStream = new MemoryStream();
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;
                using var streamReader = new StreamReader(memoryStream);
                return await streamReader.ReadToEndAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteBlob(string name) {
            var blobClient = blobContainerClient.GetBlobClient(name);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetAllBlobs(bool invalid) {
            var blobs = blobContainerClient.GetBlobsAsync();
            var jsonArray = new StringBuilder();
            // if you want to see the requests that results in invalid signature - send ?invalid=true in the request
            var getStartingTicksRegex = new Regex(@"^(\d+)");
            await foreach (var blobItem in blobs)
            {
                if (invalid && blobItem.Name.EndsWith(InvalidSignature) || !invalid && !blobItem.Name.EndsWith(InvalidSignature)) {
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    var blobResponse = await blobClient.DownloadContentAsync();
                    var content = blobResponse.Value.Content;
                    // var jsonDocument = JsonDocument.Parse(content);
                    var match = getStartingTicksRegex.Match(blobItem.Name);
                    if (match.Success) {
                        var dateTime = new DateTime(long.Parse(match.Groups[1].Value));
                        jsonArray.Append(dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + content + "\n");
                    } else {
                        jsonArray.Append(blobItem.Name + "\t" + content + "\n");
                    }
                }
            }

            return jsonArray.ToString();
        }

        public async Task<string> DeleteAllBlobs() {
            var blobs = blobContainerClient.GetBlobsAsync();
            var stringBuilder = new StringBuilder();
            await foreach (var blobItem in blobs) {
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                stringBuilder.Append(" DELETED " + blobItem.Name + "\n");
                await blobClient.DeleteAsync();
            }
            return stringBuilder.ToString();
        }
    }
}