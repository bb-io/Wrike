using Apps.Wrike.Webhooks;

namespace Tests.Wrike;

[TestClass]
public class WebhookTests : TestBase
{
    [DataTestMethod]
    [DataRow("created")]
    [DataRow("status")]
    [DataRow("custom-field")]
    public async Task SubscribeValidateUnsubscribe(string topic)
    {
        var input = new EventInput { SpaceId = Setting("SandboxSpaceId") };
        BaseWebhookHandler handler = topic switch
        {
            "created" => new TaskCreatedHandler(InvocationContext, input),
            "status" => new TaskStatusHandler(InvocationContext, input),
            _ => new TaskCustomFieldHandler(InvocationContext, input)
        };
        var values = new Dictionary<string, string> { ["payloadUrl"] = Setting("CallbackUrl") };
        try
        {
            await handler.SubscribeAsync(InvocationContext.AuthenticationCredentialsProviders, values);
            var result = await handler.ValidateSubscription(InvocationContext.AuthenticationCredentialsProviders, values);
            Assert.IsTrue(result.IsValid, result.Message);
        }
        finally
        {
            await handler.UnsubscribeAsync(InvocationContext.AuthenticationCredentialsProviders, values);
        }
    }
}
