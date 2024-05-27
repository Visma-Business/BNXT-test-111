namespace Visma.Business.WebhookExample.Exceptions;

public class ConnectWebhookException : ApplicationException {
    public ConnectWebhookException(string name, string message) : base(message) {
        Name = name;
    }

    public string Name { get; }
}