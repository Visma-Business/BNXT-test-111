using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Visma.Business.WebhookExample.Services;
using Visma.Business.WebhookExample.Tools;

namespace Visma.Business.WebhookExample
{
    public class TableChanged
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly SecretService _secretService;
        public TableChanged(BlobServiceClient blobServiceClient, SecretService secretService)
        {
            _blobServiceClient = blobServiceClient;
            _secretService = secretService;
        }

        [Function("TableChanged")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete")] HttpRequest req)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("connect-webhooks");
            BlobHelper helper = new BlobHelper(containerClient);

            try {
                if (req.Method == "GET") {
                    var allBlobs = await helper.GetAllBlobs(req.Query["invalid"] == "true");
                    var getResult = new ObjectResult(allBlobs);
                    getResult.StatusCode = StatusCodes.Status200OK;
                    return getResult;
                }
                if (req.Method == "DELETE") {
                    var allBlobs = await helper.DeleteAllBlobs();
                    var deleteResult = new ObjectResult(allBlobs);
                    deleteResult.StatusCode = StatusCodes.Status200OK;
                    return deleteResult;              
                }
                using var stream = req.Body;
                using var streamReader = new StreamReader(stream);
                var message = await streamReader.ReadToEndAsync();
                var signature = req.Headers["x-vwd-signature-v1"];
                // generate HMAC256 from the notification message
                var evt = JsonSerializer.Deserialize<VbusEvent>(message);
                if (evt == null) {
                    var noSerializeResult = new ObjectResult("Invalid Json: " + message);
                    noSerializeResult.StatusCode = StatusCodes.Status400BadRequest;
                    return noSerializeResult;
                }
                if (!_secretService.Validate(message, signature)) {
                    await helper.WriteAlert(BlobHelper.InvalidSignature, message);
                    var noValidateResult = new UnauthorizedObjectResult(message);
                    return noValidateResult;
                }
                await helper.WriteAlert(evt.Event + evt.TableIdentifier, message);
                var okResult = new OkObjectResult("Ok");
                okResult.StatusCode = StatusCodes.Status202Accepted;
                return okResult;
            } catch (Exception e) {
                await helper.WriteAlert(e.GetType().Name, e.Message);
                var errorResult = new OkObjectResult(e.Message);
                errorResult.StatusCode = StatusCodes.Status207MultiStatus;
                return errorResult;
            }
        }
    }
}
