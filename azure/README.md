# Azure functions webhook example

Azure functions are serverless and is one of the simplest ways to get started with VNXT webhooks. The azure function "TableChange" will take "post", "get", and "delete" methods. Only "post" will be used by Visma Connect to send the webhook-messages. The "post"-method will create a new blob on a predefined blob-container. The "get" method will look in the blob-container to see what posts have been made by Visma Connect. "delete" is used to delete all the blobs. 

To deploy this example to azure you can use vscode and use the azure extensions account, functions, resources and storage. 

You then have to set up an application in Visma Developer Portal (CONNECT)

Finally you configure the tables that you wish to receive notifications on.


## Azure Portal

### Blob Storage
* Go to Azure portal and select All services.
* Start typing Storage accounts in the list of resources, and select it.
* Choose Add.
* Select the subscription in which to create the storage account.
* Click on Create new under Resource group field.
* Enter a name for your new resources group.
* Navigate to your new Storage Account to see the available options for creating Blobs (Containers), File Shares, Tables, and Queues.
* Click on the “Containers” button located at the bottom of the Overview screen, then click on the “+” plus symbol next to Container.
* Choose the name "connect-webhooks" for your blob storage and click on “Create.”

### Functions App
* Choose "Create Function App in Azure...".
* Enter a globally unique name for your Function App.
* If multiple versions of your language's runtime are supported, select your desired version.
* Select a location.
* Wait for your Function App to be created.

### Visual Studio Code
* open a new workspace on the root of the checkout
* Log in to azure with azure account extension
* Right click your function app and deploy the code
* Get a connection string from your storage account by right-clicking your storage account
* Later you will also get a secret (SIGNATURE) with which we can verify the authenticity of a post. 
* Add Application Settings
  AzureWebJobsStorage = < The connection string >
  ConnectWebhookSubscriptionSignatureHashKey = < paste in the SIGNATURE from CONNECT Webhook setup >
