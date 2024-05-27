using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using Visma.Business.WebhookExample.Services;
using System.Security;

var host = new HostBuilder ()
.ConfigureFunctionsWebApplication ()
.ConfigureServices (services =>
{
    services.AddSingleton(x => new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
    SecureString sec = new SecureString();
    var signature = Environment.GetEnvironmentVariable("ConnectWebhookSubscriptionSignatureHashKey");
    if (signature != null) {
        foreach (var c in signature.ToCharArray()) {
            sec.AppendChar(c);
        } 
        sec.MakeReadOnly();
        services.AddSingleton<SecretService>(x => new SecretService(sec));
    }
})
.Build ();

host.Run ();
