# Business NXT
BNXT webhooks is a way to be notified when there are changes in the company database. 

## Your application
To use webhooks you will need an application with a public endpoit that is capable to process a POST-request and return 202-accept. Before you have this you can use a free service https://webhook.site/ to inspect the headers and content posted from BNXT. Be careful with what data is shared with third parties. For your subscription you will get a key with which you should validate each request. 

## Visma Developer Portal

* Define an application (APP) to represent your code in CONNECT. Please note that app name should not have more than 24 characters as the user table will not support longer names.
* Add a subscription from Publisher Business NXT (business), the TableChange event
* You will then get a button to copy the SIGNATUREKEY that is needed for the Functions App.

## Business NXT

Create a layout that contains the table User. From the user table, join in WebhookTarget via (User name->User name). Then from WebhookTarget join in Webhook Subscription (Webhook target->Line no.). Save changes.

Use the layout and add a user with the name of your app from (CONNECT)

Add one or more targets for this user - you may give this a descriptive name like "Order integration", "Customer registry" etc. 

Add subscriptions for the combinations of companies and tables for each of the targets. 

## Example application

The SIGNATUREKEY you get you have to use in your application to validate each message from CONNECT. It is important that you implement a validation for the messages to avoid fake data to be injected into your integration. For your convenience, we have made an [example implementation](azure/README.md) with this validation using azure functions.


